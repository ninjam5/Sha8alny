using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
               .IsRequired()
               .HasMaxLength(50)
               .HasDefaultValue("Pending");

        builder.Property(a => a.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(a => a.CV)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(a => a.Proposal)
                .IsRequired()
               .HasMaxLength(2000);

        builder.Property(a => a.Notes)
               .HasMaxLength(1000);

        builder.HasOne(a => a.Opportunity)
                .WithMany(o => o.Applications)
                .HasForeignKey(a => a.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade);
        builder.ToTable("Applications");
    }
}
