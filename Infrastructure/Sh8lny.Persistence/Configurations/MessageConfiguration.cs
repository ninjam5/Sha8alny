using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(m => m.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.UIdSender)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Receiver)
               .WithMany(u => u.ReceivedMessages)
               .HasForeignKey(m => m.UIdReceiver)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Messages");
    }
}
