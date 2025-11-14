using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Conversation entity
/// </summary>
public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        // Table mapping
        builder.ToTable("Conversations");

        // Primary key
        builder.HasKey(c => c.ConversationID);

        // Properties
        builder.Property(c => c.ConversationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.ConversationName)
            .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(c => c.ConversationType)
            .HasDatabaseName("IDX_Conversations_ConversationType");

        builder.HasIndex(c => c.GroupID)
            .HasDatabaseName("IDX_Conversations_GroupID");

        builder.HasIndex(c => c.LastMessageAt)
            .HasDatabaseName("IDX_Conversations_LastMessageAt");

        // Relationships
        // Project relationship removed from model

        builder.HasOne(c => c.Group)
            .WithMany(g => g.Conversations)
            .HasForeignKey(c => c.GroupID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Participants)
            .WithOne(cp => cp.Conversation)
            .HasForeignKey(cp => cp.ConversationID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
