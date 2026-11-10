// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> b)
    {
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(4000);
        b.Property(x => x.Location).HasMaxLength(200);
        // Store the enum as text so the column stays readable in any provider.
        b.Property(x => x.ListingType).HasConversion<string>().HasMaxLength(20);

        b.HasOne(x => x.BandProfile).WithMany(p => p.Listings).HasForeignKey(x => x.BandProfileId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Instrument).WithMany().HasForeignKey(x => x.InstrumentId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
    }
}
