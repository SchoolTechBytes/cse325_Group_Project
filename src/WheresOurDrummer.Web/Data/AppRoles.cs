namespace WheresOurDrummer.Web.Data;

/// <summary>Names of the application roles. Use these instead of string literals.</summary>
public static class AppRoles
{
    public const string Musician = "Musician";
    public const string Band = "Band";

    /// <summary>Roles a person can pick for themselves when they register.</summary>
    public static readonly string[] SelfAssignable = [Musician, Band];
}
