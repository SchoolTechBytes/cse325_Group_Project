using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheresOurDrummer.Web.Data;

namespace WheresOurDrummer.Web.Services;

/// <summary>Decides where a user lands after logging in.</summary>
public sealed class ProfileOnboardingService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager)
{
    /// <summary>
    /// Returns the profile-creation page for the user's role when they have no profile yet,
    /// or null when no setup is needed (profile exists, or the user has neither role).
    /// </summary>
    public async Task<string?> GetProfileSetupUrlAsync(ApplicationUser user)
    {
        if (await userManager.IsInRoleAsync(user, AppRoles.Musician)
            && !await db.MusicianProfiles.AnyAsync(p => p.UserId == user.Id))
        {
            return ProfileRoutes.CreateMusician;
        }

        if (await userManager.IsInRoleAsync(user, AppRoles.Band)
            && !await db.BandProfiles.AnyAsync(p => p.UserId == user.Id))
        {
            return ProfileRoutes.CreateBand;
        }

        return null;
    }

    /// <summary>Profile setup takes priority; otherwise go where the user was headed.</summary>
    public async Task<string?> GetLandingUrlAsync(ApplicationUser user, string? returnUrl) =>
        await GetProfileSetupUrlAsync(user) ?? returnUrl;
}
