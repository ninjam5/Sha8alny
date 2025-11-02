using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Message entity
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Primary key
        builder.HasKey(m => m.Id);

        // Required properties with max lengths
        builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);

        // Timestamp with database default
        builder.Property(m => m.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Sender relationship (RESTRICT to preserve user data)
        builder.HasOne(m => m.Sender)
               .WithMany(u => u.SentMessages)
               .HasForeignKey(m => m.UIdSender)
               .OnDelete(DeleteBehavior.Restrict);

        // Receiver relationship (RESTRICT to preserve user data)
        builder.HasOne(m => m.Receiver)
               .WithMany(u => u.ReceivedMessages)
               .HasForeignKey(m => m.UIdReceiver)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Messages");
    }
}
