using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
               .ValueGeneratedOnAdd();

        builder.Property(o => o.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(o => o.Type)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(o => o.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(o => o.Deadline)
               .IsRequired()
               .HasColumnType("datetime2");

        builder.Property(o => o.Is_Paid)
               .IsRequired()
               .HasDefaultValue(false);

       
        builder.Property(o => o.Payment)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0);

        builder.Property(o => o.Duration)
               .HasMaxLength(100);

        builder.Property(o => o.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.Library)
               .HasMaxLength(2000)
               .HasComment("JSON array of library resources");

        builder.HasIndex(o => o.Type);

        builder.HasIndex(o => o.Deadline);

        builder.ToTable("Opportunities");
    }
}
