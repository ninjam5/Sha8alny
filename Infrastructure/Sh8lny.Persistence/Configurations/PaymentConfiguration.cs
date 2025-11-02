using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Payment entity
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Payment amount with precision
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");

        // Required properties with max lengths and defaults
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
        builder.Property(p => p.Type).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Method).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");

        // Timestamp with database default
        builder.Property(p => p.Paid_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Sender relationship (RESTRICT to preserve user data)
        builder.HasOne(p => p.Sender)
               .WithMany(u => u.SentPayments)
               .HasForeignKey(p => p.UIdSender)
               .OnDelete(DeleteBehavior.Restrict);

        // Receiver relationship (RESTRICT to preserve user data)
        builder.HasOne(p => p.Receiver)
               .WithMany(u => u.ReceivedPayments)
               .HasForeignKey(p => p.UIdReceiver)
               .OnDelete(DeleteBehavior.Restrict);

        // CompletedOpportunity relationship (RESTRICT for data integrity)
        builder.HasOne(p => p.CompletedOpportunity)
               .WithMany(co => co.Payments)
               .HasForeignKey(p => p.Completed_id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Payments");
    }
}
