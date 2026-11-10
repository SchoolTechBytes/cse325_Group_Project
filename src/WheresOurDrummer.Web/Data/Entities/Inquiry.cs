// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Entities;

public class Inquiry
{
    public int Id { get; set; }
    public string FromUserId { get; set; } = string.Empty;
    public ApplicationUser? FromUser { get; set; }
    public string ToUserId { get; set; } = string.Empty;
    public ApplicationUser? ToUser { get; set; }

    // Optional context: the message is about a listing or a musician profile.
    public int? ListingId { get; set; }
    public Listing? Listing { get; set; }
    public int? MusicianProfileId { get; set; }
    public MusicianProfile? MusicianProfile { get; set; }

    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
