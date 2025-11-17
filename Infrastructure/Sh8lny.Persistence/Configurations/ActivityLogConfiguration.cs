using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for ActivityLog entity
/// </summary>
public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        // Table mapping
        builder.ToTable("ActivityLog");

        // Primary key
        builder.HasKey(al => al.LogID);

        // Properties
        builder.Property(al => al.ActivityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.Description)
            .HasMaxLength(1000);

        builder.Property(al => al.RelatedEntityType)
            .HasMaxLength(50);

        builder.Property(al => al.IPAddress)
            .HasMaxLength(45);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.Property(al => al.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(al => al.UserID)
            .HasDatabaseName("IDX_ActivityLog_UserID");

        builder.HasIndex(al => al.ActivityType)
            .HasDatabaseName("IDX_ActivityLog_ActivityType");

        builder.HasIndex(al => al.CreatedAt)
            .HasDatabaseName("IDX_ActivityLog_CreatedAt");

        builder.HasIndex(al => new { al.RelatedEntityType, al.RelatedEntityID })
            .HasDatabaseName("IDX_ActivityLog_RelatedEntity");
    }
}
