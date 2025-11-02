using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Opportunity entity
public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        // Primary key
        builder.HasKey(o => o.Id);

        // Required text properties with max lengths
        builder.Property(o => o.Title).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Type).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Description).IsRequired().HasMaxLength(2000);
        builder.Property(o => o.Duration).HasMaxLength(100);

        // Date and time properties
        builder.Property(o => o.Deadline).IsRequired().HasColumnType("datetime2");
        builder.Property(o => o.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Payment properties with defaults
        builder.Property(o => o.Is_Paid).IsRequired().HasDefaultValue(false);
        builder.Property(o => o.Payment).HasColumnType("decimal(18,2)").HasDefaultValue(0);

        // One-to-one relationship with completed opportunity
        builder.HasOne(o => o.CompletedOpportunity)
               .WithOne(co => co.Opportunity)
               .HasForeignKey<CompletedOpportunity>(co => co.OpportunityId);

        builder.ToTable("Opportunities");
    }
}
