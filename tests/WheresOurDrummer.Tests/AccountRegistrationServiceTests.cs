using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WheresOurDrummer.Web.Data;
using WheresOurDrummer.Web.Data.Entities;
using WheresOurDrummer.Web.Data.Seed;
using WheresOurDrummer.Web.Services;

namespace WheresOurDrummer.Tests;

public class AccountRegistrationServiceTests : IAsyncLifetime
{
    private const string Password = "Test-Passw0rd!";

    // An in-memory SQLite database lives only as long as its connection, so keep it open for the test.
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
        services.AddScoped<AccountRegistrationService>();
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

    private static ApplicationUser NewUser(string email) => new() { UserName = email, Email = email };

    [Theory]
    [InlineData(AppRoles.Musician)]
    [InlineData(AppRoles.Band)]
    public async Task CreateWithRoleAsync_StoresChosenRole(string role)
    {
        using var scope = _provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<AccountRegistrationService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var result = await service.CreateWithRoleAsync(NewUser("new@example.test"), Password, role);

        Assert.True(result.Succeeded);
        var saved = await userManager.FindByEmailAsync("new@example.test");
        Assert.NotNull(saved);
        Assert.Equal([role], await userManager.GetRolesAsync(saved));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Admin")]
    [InlineData("musician")]
    public async Task CreateWithRoleAsync_RejectsRoleNotOnTheList_AndCreatesNoUser(string role)
    {
        using var scope = _provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<AccountRegistrationService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var result = await service.CreateWithRoleAsync(NewUser("bad@example.test"), Password, role);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "InvalidRole");
        Assert.Null(await userManager.FindByEmailAsync("bad@example.test"));
    }

    [Fact]
    public async Task CreateWithRoleAsync_WhenRoleIsMissingFromDatabase_RollsBackUser()
    {
        using var scope = _provider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await roleManager.DeleteAsync((await roleManager.FindByNameAsync(AppRoles.Band))!);

        var service = scope.ServiceProvider.GetRequiredService<AccountRegistrationService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Identity throws when the role row doesn't exist; the user must not be left behind.
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateWithRoleAsync(NewUser("orphan@example.test"), Password, AppRoles.Band));

        Assert.Null(await userManager.FindByEmailAsync("orphan@example.test"));
    }
}
