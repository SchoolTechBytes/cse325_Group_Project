// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class SampleLink
{
    public int Id { get; set; }
    public int MusicianProfileId { get; set; }
    public MusicianProfile? MusicianProfile { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
