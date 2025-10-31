using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class CompletedOpportunityConfiguration : IEntityTypeConfiguration<CompletedOpportunity>
{
    public void Configure(EntityTypeBuilder<CompletedOpportunity> builder)
    {
        builder.HasKey(co => co.Id);
        builder.Property(co => co.ConfirmedByStudent)
               .IsRequired();

        builder.Property(co => co.ConfirmedByCompany)
               .IsRequired();

        builder.Property(co => co.ConfirmedByPayment)
               .IsRequired();
        
        builder.ToTable("CompletedOpportunities");
    }
}
