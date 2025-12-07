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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table mapping
            builder.ToTable("Notifications");

            // Primary key
            builder.HasKey(n => n.NotificationID);

            // Properties
            builder.Property(n => n.NotificationType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.ActionURL)
                .HasMaxLength(500);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Indexes
            builder.HasIndex(n => n.UserID)
                .HasDatabaseName("IDX_Notifications_UserID");

            builder.HasIndex(n => n.IsRead)
                .HasDatabaseName("IDX_Notifications_IsRead");

            builder.HasIndex(n => n.NotificationType)
                .HasDatabaseName("IDX_Notifications_NotificationType");

            builder.HasIndex(n => n.CreatedAt)
                .HasDatabaseName("IDX_Notifications_CreatedAt");
        }
    }
}
