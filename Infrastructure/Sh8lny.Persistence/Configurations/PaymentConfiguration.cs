using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .ValueGeneratedOnAdd();

        builder.Property(p => p.Amount)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
               .IsRequired()
               .HasMaxLength(3);

        builder.Property(p => p.Type)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(p => p.Method)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(p => p.Status)
               .IsRequired()
               .HasMaxLength(50)
               .HasDefaultValue("Pending");

        builder.Property(p => p.Paid_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.Transaction_Id)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(p => p.Transaction_Id)
               .IsUnique();

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.Paid_At);

        builder.ToTable("Payments");
    }
}
