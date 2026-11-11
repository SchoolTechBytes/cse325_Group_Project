// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<MusicianProfile> Musicians { get; set; } = [];
    public List<BandProfile> Bands { get; set; } = [];
}
