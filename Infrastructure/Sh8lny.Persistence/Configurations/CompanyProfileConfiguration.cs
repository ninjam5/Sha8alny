using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for CompanyProfile entity
public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        // Primary key
        builder.HasKey(cp => cp.Id);

        // Required properties with max lengths
        builder.Property(cp => cp.Industry).IsRequired().HasMaxLength(100);

        // One-to-many relationship with opportunities (CASCADE delete)
        builder.HasMany(c => c.Opportunities)
               .WithOne(o => o.CompanyProfile)
               .HasForeignKey(o => o.CompanyProfileId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("CompanyProfiles");
    }
}
