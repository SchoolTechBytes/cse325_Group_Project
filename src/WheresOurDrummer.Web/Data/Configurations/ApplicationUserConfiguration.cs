// DRAFT until the group agrees the data model.
namespace WheresOurDrummer.Web.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> b) =>
        b.Property(x => x.DisplayName).HasMaxLength(100);
}
