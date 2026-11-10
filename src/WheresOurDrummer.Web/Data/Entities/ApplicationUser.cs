// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

using Microsoft.AspNetCore.Identity;

/// <summary>Login account. Extends the Identity user with a public display name.</summary>
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
