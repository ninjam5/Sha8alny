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
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            // Table mapping
            builder.ToTable("UserSettings");

            // Primary key
            builder.HasKey(us => us.SettingID);

            // Properties
            builder.Property(us => us.EmailNotifications)
                .HasDefaultValue(true);

            builder.Property(us => us.PushNotifications)
                .HasDefaultValue(true);

            builder.Property(us => us.MessageNotifications)
                .HasDefaultValue(true);

            builder.Property(us => us.ApplicationNotifications)
                .HasDefaultValue(true);

            builder.Property(us => us.Language)
                .HasMaxLength(50)
                .HasDefaultValue("English");

            builder.Property(us => us.Timezone)
                .HasMaxLength(50)
                .HasDefaultValue("UTC");

            builder.Property(us => us.ProfileVisibility)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>()
                .HasDefaultValue(ProfileVisibility.Public);

            builder.Property(us => us.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Indexes
            builder.HasIndex(us => us.UserID)
                .IsUnique()
                .HasDatabaseName("IDX_UserSettings_UserID");
        }
    }
}
