using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class CompletedOpportunityConfiguration : IEntityTypeConfiguration<CompletedOpportunity>
{
    public void Configure(EntityTypeBuilder<CompletedOpportunity> builder)
    {
        builder.HasKey(co => co.Id);

        builder.Property(co => co.Id)
               .ValueGeneratedOnAdd();

        builder.Property(co => co.CompletedBy)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(co => co.AcceptedBy)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(co => co.SubmitContains)
               .HasMaxLength(2000);

    
        builder.Property(co => co.Payment)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(co => co.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(co => co.CompletedBy);

        builder.HasIndex(co => co.AcceptedBy);

        builder.ToTable("CompletedOpportunities");
    }
}
