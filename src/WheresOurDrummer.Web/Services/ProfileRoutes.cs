namespace WheresOurDrummer.Web.Services;

/// <summary>
/// Where new users are sent to create their first profile. Slice B owns the musician page
/// and Slice C owns the band page; keep their @page routes in sync with these constants.
/// </summary>
public static class ProfileRoutes
{
    public const string CreateMusician = "musicians/create";
    public const string CreateBand = "bands/create";
}
