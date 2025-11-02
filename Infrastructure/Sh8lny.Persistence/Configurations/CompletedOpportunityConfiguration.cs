using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for CompletedOpportunity entity
public class CompletedOpportunityConfiguration : IEntityTypeConfiguration<CompletedOpportunity>
{
    public void Configure(EntityTypeBuilder<CompletedOpportunity> builder)
    {
        // Primary key
        builder.HasKey(co => co.Id);

        // Boolean confirmation flags
        builder.Property(co => co.ConfirmedByStudent).IsRequired();
        builder.Property(co => co.ConfirmedByCompany).IsRequired();
        builder.Property(co => co.ConfirmedByPayment).IsRequired();
        
        builder.ToTable("CompletedOpportunities");
    }
}
