// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MusicianProfileConfiguration : IEntityTypeConfiguration<MusicianProfile>
{
    public void Configure(EntityTypeBuilder<MusicianProfile> b)
    {
        b.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
        b.Property(x => x.Bio).HasMaxLength(2000);
        b.Property(x => x.Location).HasMaxLength(200);
        b.Property(x => x.ExperienceLevel).HasMaxLength(50);

        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Instruments).WithMany(i => i.Musicians);
        b.HasMany(x => x.Genres).WithMany(g => g.Musicians);
    }
}
