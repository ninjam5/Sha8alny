using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for GroupMember entity
/// </summary>
public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        // Table mapping
        builder.ToTable("GroupMembers");

        // Primary key
        builder.HasKey(gm => gm.GroupMemberID);

        // Alternate key - a student can only be in a group once
        builder.HasIndex(gm => new { gm.GroupID, gm.StudentID })
            .IsUnique()
            .HasDatabaseName("IDX_GroupMembers_GroupID_StudentID");

        // Properties
        builder.Property(gm => gm.Role)
            .HasMaxLength(50);

        builder.Property(gm => gm.JoinedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(gm => gm.GroupID)
            .HasDatabaseName("IDX_GroupMembers_GroupID");

        builder.HasIndex(gm => gm.StudentID)
            .HasDatabaseName("IDX_GroupMembers_StudentID");
    }
}
