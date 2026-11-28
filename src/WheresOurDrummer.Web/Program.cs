using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheresOurDrummer.Web.Components;
using WheresOurDrummer.Web.Components.Account;
using WheresOurDrummer.Web.Data;
using WheresOurDrummer.Web.Data.Seed;
using WheresOurDrummer.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Hosting platforms (Render) tell us the port via PORT. Otherwise keep any URLs already
// configured (launchSettings, ASPNETCORE_URLS, HTTP_PORTS) and fall back to 8080.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
else if (string.IsNullOrEmpty(builder.Configuration["urls"])
    && string.IsNullOrEmpty(builder.Configuration["HTTP_PORTS"])
    && string.IsNullOrEmpty(builder.Configuration["HTTPS_PORTS"]))
{
    builder.WebHost.UseUrls("http://0.0.0.0:8080");
}

// We run behind Render's reverse proxy, which terminates TLS and sets X-Forwarded-* headers.
// Trust them (clearing the loopback-only defaults) so the app sees the real scheme and client IP.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHealthChecks();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// DATABASE PROVIDER: this is the only place that chooses the provider (PostgreSQL only).
// Changing it needs group agreement. The connection string comes from DATABASE_URL when
// set (Render), otherwise ConnectionStrings:DefaultConnection (appsettings.Development.json
// or user-secrets); never commit real credentials.
var connectionString = ConnectionStringResolver.Resolve(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Persist data protection keys so restarts/redeploys don't invalidate login cookies.
builder.Services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>() // enables RoleManager for the Musician/Band roles
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<AccountRegistrationService>();

var app = builder.Build();

// Apply migrations only when explicitly requested with RUN_MIGRATIONS=true.
// Seed roles and demo accounts in Development and Demo after the schema exists.
var runMigrations = string.Equals(
    Environment.GetEnvironmentVariable("RUN_MIGRATIONS"),
    "true",
    StringComparison.OrdinalIgnoreCase);

var seedDemoAccounts =
    app.Environment.IsDevelopment() ||
    app.Environment.IsEnvironment("Demo");

if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}
else
{
    app.Logger.LogInformation(
        "RUN_MIGRATIONS is not 'true': skipping migrations.");
}

if (seedDemoAccounts || runMigrations)
{
    using var scope = app.Services.CreateScope();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedAsync(roleManager);

    if (seedDemoAccounts)
    {
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        await DemoAccountSeeder.SeedAsync(userManager, app.Configuration);
    }
}

// Must run first so scheme/IP are corrected before anything else looks at them.
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// Redirect only in Development. In production Render already forces HTTPS at its edge, and the
// container only listens on plain HTTP, so redirecting here would loop or have no HTTPS port.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapHealthChecks("/healthz");

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
