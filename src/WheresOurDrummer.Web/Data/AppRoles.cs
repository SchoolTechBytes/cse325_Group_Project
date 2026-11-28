namespace WheresOurDrummer.Web.Data;

/// <summary>Names of the account roles. Use these instead of string literals.</summary>
public static class AppRoles
{
    public const string Musician = "Musician";
    public const string Band = "Band";

    /// <summary>Roles a user can pick for themselves at sign-up.</summary>
    public static readonly IReadOnlyList<string> SelfAssignable = [Musician, Band];
}
