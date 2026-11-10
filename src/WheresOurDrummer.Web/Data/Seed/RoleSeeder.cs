using Microsoft.AspNetCore.Identity;

namespace WheresOurDrummer.Web.Data.Seed;

/// <summary>Ensures the application roles exist. Safe to run on every startup.</summary>
public static class RoleSeeder
{
    public static readonly string[] RoleNames = ["Musician", "Band"];

    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var name in RoleNames)
        {
            // Check first so repeated runs never try to insert a duplicate.
            if (!await roleManager.RoleExistsAsync(name))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(name));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role '{name}': {string.Join("; ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
