using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Industry)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasMany(c => c.Opportunities)
                   .WithOne(o => o.CompanyProfile)
                   .HasForeignKey(o => o.CompanyProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("CompanyProfiles");
    }
}
