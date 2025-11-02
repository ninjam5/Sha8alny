using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Notification entity
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Primary key
        builder.HasKey(n => n.Id);

        // Required properties with max lengths
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Body).IsRequired().HasMaxLength(1000);

        // Read status flag with default
        builder.Property(n => n.Is_Read).IsRequired().HasDefaultValue(false);

        // Timestamp with database default
        builder.Property(n => n.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.ToTable("Notifications");
    }
}
