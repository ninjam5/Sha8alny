using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Message entity
/// </summary>
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Table mapping
        builder.ToTable("Messages");

        // Primary key
        builder.HasKey(m => m.MessageID);

        // Properties
        builder.Property(m => m.MessageText)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(m => m.MessageType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(MessageType.Text);

        builder.Property(m => m.AttachmentURL)
            .HasMaxLength(500);

        builder.Property(m => m.AttachmentName)
            .HasMaxLength(200);

        builder.Property(m => m.IsRead)
            .HasDefaultValue(false);

        builder.Property(m => m.IsEdited)
            .HasDefaultValue(false);

        builder.Property(m => m.SentAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(m => m.ConversationID)
            .HasDatabaseName("IDX_Messages_ConversationID");

        builder.HasIndex(m => m.SenderID)
            .HasDatabaseName("IDX_Messages_SenderID");

        builder.HasIndex(m => m.SentAt)
            .HasDatabaseName("IDX_Messages_SentAt");

        builder.HasIndex(m => m.IsRead)
            .HasDatabaseName("IDX_Messages_IsRead");

        // Relationships
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
