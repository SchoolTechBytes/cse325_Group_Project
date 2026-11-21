using WheresOurDrummer.Web.Data.Entities;

namespace WheresOurDrummer.Web.Data.Seed;

/// <summary>
/// Seed data for the Instrument and Genre lookup tables (applied via HasData).
/// Ids are fixed so migrations stay stable. Only append new entries with new Ids;
/// never renumber or reuse an Id.
/// </summary>
public static class LookupSeedData
{
    private static readonly string[] InstrumentNames =
    [
        "Acoustic Guitar",
        "Electric Guitar",
        "Bass Guitar",
        "Upright Bass",
        "Drums",
        "Percussion",
        "Vocals",
        "Backing Vocals",
        "Piano",
        "Keyboards",
        "Synthesizer",
        "Organ",
        "Violin",
        "Viola",
        "Cello",
        "Banjo",
        "Mandolin",
        "Ukulele",
        "Harmonica",
        "Saxophone",
        "Trumpet",
        "Trombone",
        "French Horn",
        "Tuba",
        "Clarinet",
        "Flute",
        "Pedal Steel",
        "DJ / Turntables",
        "Producer / Programming",
        "Other",
    ];

    private static readonly string[] GenreNames =
    [
        "Rock",
        "Alternative",
        "Indie",
        "Punk",
        "Metal",
        "Hard Rock",
        "Pop",
        "Country",
        "Folk",
        "Bluegrass",
        "Blues",
        "Jazz",
        "Funk",
        "Soul",
        "R&B",
        "Hip-Hop",
        "Reggae",
        "Latin",
        "Gospel",
        "Worship",
        "Classical",
        "Electronic",
        "Ska",
        "Cover Band",
        "Singer-Songwriter",
        "Other",
    ];

    // Ids are positional (1-based), so keep the arrays above append-only.
    public static IReadOnlyList<Instrument> Instruments { get; } =
        InstrumentNames.Select((name, i) => new Instrument { Id = i + 1, Name = name }).ToArray();

    public static IReadOnlyList<Genre> Genres { get; } =
        GenreNames.Select((name, i) => new Genre { Id = i + 1, Name = name }).ToArray();
}
