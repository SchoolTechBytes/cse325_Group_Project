// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> b)
    {
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.Name).IsUnique();
    }
}
