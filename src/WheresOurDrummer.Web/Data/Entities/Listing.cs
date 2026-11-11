// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class Listing
{
    public int Id { get; set; }
    public int BandProfileId { get; set; }
    public BandProfile? BandProfile { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public ListingType ListingType { get; set; }
    public int InstrumentId { get; set; }
    public Instrument? Instrument { get; set; }
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    public bool IsOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
