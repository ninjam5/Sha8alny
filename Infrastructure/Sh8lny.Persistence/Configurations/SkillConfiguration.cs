using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Skill entity
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        // Table mapping
        builder.ToTable("Skills");

        // Primary key
        builder.HasKey(s => s.SkillID);

        // Properties
        builder.Property(s => s.SkillName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.SkillName)
            .IsUnique()
            .HasDatabaseName("UQ_Skills_SkillName");

        builder.Property(s => s.SkillCategory)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(s => s.SkillCategory)
            .HasDatabaseName("IDX_Skills_SkillCategory");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IDX_Skills_IsActive");
    }
}
