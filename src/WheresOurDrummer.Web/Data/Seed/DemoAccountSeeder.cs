using Microsoft.AspNetCore.Identity;
using WheresOurDrummer.Web.Data.Entities;

namespace WheresOurDrummer.Web.Data.Seed;

/// <summary>Creates the non-production accounts used for demos.</summary>
public static class DemoAccountSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var musicianPassword = GetPassword(configuration, "MusicianPassword");
        var bandPassword = GetPassword(configuration, "BandPassword");

        await EnsureAccountAsync(
            userManager,
            "demo.musician@example.test",
            "Demo Musician",
            musicianPassword,
            "Musician");

        await EnsureAccountAsync(
            userManager,
            "demo.band@example.test",
            "Demo Band",
            bandPassword,
            "Band");
    }

    private static string GetPassword(IConfiguration configuration, string name) =>
        configuration[$"DemoAccounts:{name}"]
        ?? throw new InvalidOperationException(
            $"Missing DemoAccounts:{name} configuration. Set it using User Secrets for local development.");

    private static async Task EnsureAccountAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string displayName,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not create demo account '{email}': " +
                    string.Join("; ", createResult.Errors.Select(error => error.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not assign role '{role}' to '{email}': " +
                    string.Join("; ", roleResult.Errors.Select(error => error.Description)));
            }
        }
    }
}