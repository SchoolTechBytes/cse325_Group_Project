// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SampleLinkConfiguration : IEntityTypeConfiguration<SampleLink>
{
    public void Configure(EntityTypeBuilder<SampleLink> b)
    {
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.Property(x => x.Url).IsRequired().HasMaxLength(500);

        b.HasOne(x => x.MusicianProfile).WithMany(p => p.SampleLinks).HasForeignKey(x => x.MusicianProfileId).OnDelete(DeleteBehavior.Cascade);
    }
}
