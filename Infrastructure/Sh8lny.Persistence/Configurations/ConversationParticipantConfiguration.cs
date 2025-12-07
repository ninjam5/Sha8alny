using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations
{
    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            // Table mapping
            builder.ToTable("ConversationParticipants");

            // Primary key
            builder.HasKey(cp => cp.ParticipantID);

            // Alternate key - a user can only be in a conversation once
            builder.HasIndex(cp => new { cp.ConversationID, cp.UserID })
                .IsUnique()
                .HasDatabaseName("IDX_ConversationParticipants_ConversationID_UserID");

            // Properties
            builder.Property(cp => cp.JoinedAt)
                .HasDefaultValueSql("GETDATE()");

            // Indexes
            builder.HasIndex(cp => cp.ConversationID)
                .HasDatabaseName("IDX_ConversationParticipants_ConversationID");

            builder.HasIndex(cp => cp.UserID)
                .HasDatabaseName("IDX_ConversationParticipants_UserID");
        }
    }
}
