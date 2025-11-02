using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for User entity
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Required properties with max lengths
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(500);
        builder.Property(u => u.Role).IsRequired().HasMaxLength(50);

        // Optional properties with max lengths
        builder.Property(u => u.ProfilePicture).HasMaxLength(500);
        builder.Property(u => u.Bio).HasMaxLength(1000);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);

        // Timestamp with database default
        builder.Property(u => u.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // One-to-one relationships with profiles (CASCADE delete)
        builder.HasOne(u => u.StudentProfile)
               .WithOne(s => s.User)
               .HasForeignKey<StudentProfile>(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.CompanyProfile)
               .WithOne(s => s.User)
               .HasForeignKey<CompanyProfile>(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // One-to-many relationship with notifications (CASCADE delete)
        builder.HasMany(u => u.Notification)
               .WithOne(n => n.User)
               .HasForeignKey(n => n.UId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Users");
    }
}
