using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WheresOurDrummer.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{
    public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();
    public DbSet<BandProfile> BandProfiles => Set<BandProfile>();
    public DbSet<Instrument> Instruments => Set<Instrument>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<SampleLink> SampleLinks => Set<SampleLink>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();

    // Auth cookie keys live in the DB because Render's filesystem is ephemeral.
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Picks up every IEntityTypeConfiguration<T> in Data/Configurations, so teammates
        // add a new file instead of editing this one.
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
