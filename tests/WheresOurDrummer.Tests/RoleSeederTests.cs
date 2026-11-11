using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WheresOurDrummer.Web.Data;
using WheresOurDrummer.Web.Data.Entities;
using WheresOurDrummer.Web.Data.Seed;

namespace WheresOurDrummer.Tests;

public class RoleSeederTests : IAsyncLifetime
{
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
        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _provider.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task SeedAsync_CreatesBothRoles_AndIsSafeToRunTwice()
    {
        // Run twice, each in a fresh scope, to mimic two application startups.
        for (var i = 0; i < 2; i++)
        {
            using var scope = _provider.CreateScope();
            await RoleSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());
        }

        using var verify = _provider.CreateScope();
        var names = await verify.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Roles.Select(r => r.Name).ToListAsync();

        Assert.Equal(2, names.Count);
        Assert.Contains("Musician", names);
        Assert.Contains("Band", names);
    }
}
