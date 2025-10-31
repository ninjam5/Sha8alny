using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .ValueGeneratedOnAdd();

        builder.Property(u => u.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
               .IsUnique();

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

        builder.ToTable("Users");
    }
}
