using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.Password)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.ProfilePicture)
               .HasMaxLength(500);

        builder.Property(u => u.Bio)
               .HasMaxLength(1000);

        builder.Property(u => u.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(u => u.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
        builder.HasOne(u=>u.StudentProfile)
            .WithOne(s=>s.User)
            .HasForeignKey<StudentProfile>(s=>s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(u => u.CompanyProfile)
            .WithOne(s => s.User)
            .HasForeignKey<CompanyProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notification)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Users");
    }
}
