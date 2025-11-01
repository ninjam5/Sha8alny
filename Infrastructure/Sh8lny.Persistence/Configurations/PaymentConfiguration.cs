using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

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

        builder.HasOne(p => p.Sender)
                   .WithMany(u => u.SentPayments)
                   .HasForeignKey(p => p.UIdSender)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Receiver)
               .WithMany(u => u.ReceivedPayments)
               .HasForeignKey(p => p.UIdReceiver)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CompletedOpportunity)
               .WithMany(co => co.Payments)
               .HasForeignKey(p => p.Completed_id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Payments");
    }
}
