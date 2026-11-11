// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class MusicianProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? ExperienceLevel { get; set; }
    public DateTime CreatedAt { get; set; }

    // Many-to-many: EF creates the join tables from these skip navigations.
    public List<Instrument> Instruments { get; set; } = [];
    public List<Genre> Genres { get; set; } = [];
    public List<SampleLink> SampleLinks { get; set; } = [];
}
