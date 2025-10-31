using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
               .ValueGeneratedOnAdd();

        builder.Property(n => n.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(n => n.Body)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(n => n.Is_Read)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(n => n.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(n => n.Is_Read);

        builder.HasIndex(n => n.Created_At);

        builder.ToTable("Notifications");
    }
}
