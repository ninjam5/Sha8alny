using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for University entity
/// </summary>
public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        // Table mapping
        builder.ToTable("Universities");

        // Primary key
        builder.HasKey(u => u.UniversityID);

        // Properties
        builder.Property(u => u.UniversityName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.UniversityLogo)
            .HasMaxLength(500);

        builder.Property(u => u.ContactEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.ContactPhone)
            .HasMaxLength(20);

        builder.Property(u => u.Website)
            .HasMaxLength(500);

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        builder.Property(u => u.City)
            .HasMaxLength(100);

        builder.Property(u => u.Country)
            .HasMaxLength(100);

        builder.Property(u => u.UniversityType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(u => u.UserID)
            .HasDatabaseName("IDX_Universities_UserID");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IDX_Universities_IsActive");
    }
}
