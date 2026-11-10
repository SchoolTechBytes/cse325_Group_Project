using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WheresOurDrummer.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<MusicianProfile> MusicianProfiles => Set<MusicianProfile>();
    public DbSet<BandProfile> BandProfiles => Set<BandProfile>();
    public DbSet<Instrument> Instruments => Set<Instrument>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<SampleLink> SampleLinks => Set<SampleLink>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Picks up every IEntityTypeConfiguration<T> in Data/Configurations, so teammates
        // add a new file instead of editing this one.
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
