using Microsoft.AspNetCore.Identity;
using WheresOurDrummer.Web.Data;

namespace WheresOurDrummer.Web.Services;

/// <summary>Creates a new account and stores the role the user picked at sign-up.</summary>
public class AccountRegistrationService(UserManager<ApplicationUser> userManager)
{
    public async Task<IdentityResult> CreateWithRoleAsync(ApplicationUser user, string password, string role)
    {
        // Never trust the posted value: only the self-assignable roles are allowed.
        if (!AppRoles.SelfAssignable.Contains(role))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = "Choose whether you are registering as a musician or a band."
            });
        }

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return createResult;
        }

        // Don't leave an account behind with no role. AddToRoleAsync returns a failed result
        // for most problems but throws if the role row is missing (roles not seeded).
        IdentityResult roleResult;
        try
        {
            roleResult = await userManager.AddToRoleAsync(user, role);
        }
        catch
        {
            await userManager.DeleteAsync(user);
            throw;
        }

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return roleResult;
        }

        return IdentityResult.Success;
    }
}
