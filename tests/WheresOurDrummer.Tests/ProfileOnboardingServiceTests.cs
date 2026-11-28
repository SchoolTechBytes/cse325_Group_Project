using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WheresOurDrummer.Web.Data;
using WheresOurDrummer.Web.Data.Entities;
using WheresOurDrummer.Web.Data.Seed;
using WheresOurDrummer.Web.Services;

namespace WheresOurDrummer.Tests;

public class ProfileOnboardingServiceTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private ServiceProvider _provider = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(_connection));
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<ProfileOnboardingService>();
        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreatedAsync();
        await RoleSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());
    }

    public async Task DisposeAsync()
    {
        await _provider.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private async Task<ApplicationUser> CreateUserAsync(IServiceProvider sp, string? role)
    {
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var email = $"{Guid.NewGuid():N}@example.test";
        var user = new ApplicationUser { UserName = email, Email = email };
        Assert.True((await userManager.CreateAsync(user)).Succeeded);
        if (role is not null)
        {
            Assert.True((await userManager.AddToRoleAsync(user, role)).Succeeded);
        }
        return user;
    }

    [Theory]
    [InlineData(AppRoles.Musician, ProfileRoutes.CreateMusician)]
    [InlineData(AppRoles.Band, ProfileRoutes.CreateBand)]
    public async Task NewUserWithoutProfile_IsSentToProfileCreationForTheirRole(string role, string expected)
    {
        using var scope = _provider.CreateScope();
        var user = await CreateUserAsync(scope.ServiceProvider, role);
        var service = scope.ServiceProvider.GetRequiredService<ProfileOnboardingService>();

        Assert.Equal(expected, await service.GetProfileSetupUrlAsync(user));
        // Profile setup wins over wherever the user was headed.
        Assert.Equal(expected, await service.GetLandingUrlAsync(user, "inbox"));
    }

    [Fact]
    public async Task MusicianWithProfile_GoesToReturnUrl()
    {
        using var scope = _provider.CreateScope();
        var sp = scope.ServiceProvider;
        var user = await CreateUserAsync(sp, AppRoles.Musician);
        var db = sp.GetRequiredService<ApplicationDbContext>();
        db.MusicianProfiles.Add(new MusicianProfile { UserId = user.Id, DisplayName = "Alex" });
        await db.SaveChangesAsync();

        var service = sp.GetRequiredService<ProfileOnboardingService>();

        Assert.Null(await service.GetProfileSetupUrlAsync(user));
        Assert.Equal("inbox", await service.GetLandingUrlAsync(user, "inbox"));
    }

    [Fact]
    public async Task BandWithProfile_GoesToReturnUrl()
    {
        using var scope = _provider.CreateScope();
        var sp = scope.ServiceProvider;
        var user = await CreateUserAsync(sp, AppRoles.Band);
        var db = sp.GetRequiredService<ApplicationDbContext>();
        db.BandProfiles.Add(new BandProfile { UserId = user.Id, Name = "Midnight Echo" });
        await db.SaveChangesAsync();

        var service = sp.GetRequiredService<ProfileOnboardingService>();

        Assert.Null(await service.GetLandingUrlAsync(user, null));
    }

    [Fact]
    public async Task UserWithNoRole_IsNotRedirected()
    {
        using var scope = _provider.CreateScope();
        var user = await CreateUserAsync(scope.ServiceProvider, role: null);
        var service = scope.ServiceProvider.GetRequiredService<ProfileOnboardingService>();

        Assert.Equal("listings", await service.GetLandingUrlAsync(user, "listings"));
    }
}
