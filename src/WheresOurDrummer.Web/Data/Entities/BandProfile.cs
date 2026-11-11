// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class BandProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }

    public List<Genre> Genres { get; set; } = [];
    public List<Listing> Listings { get; set; } = [];
}
