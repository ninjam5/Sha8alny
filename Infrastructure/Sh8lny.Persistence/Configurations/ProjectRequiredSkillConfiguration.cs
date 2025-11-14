using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for ProjectRequiredSkill junction table
/// </summary>
public class ProjectRequiredSkillConfiguration : IEntityTypeConfiguration<ProjectRequiredSkill>
{
    public void Configure(EntityTypeBuilder<ProjectRequiredSkill> builder)
    {
        // Table mapping
        builder.ToTable("ProjectRequiredSkills");

        // Primary key
        builder.HasKey(prs => prs.ProjectSkillID);

        // Unique constraint
        builder.HasIndex(prs => new { prs.ProjectID, prs.SkillID })
            .IsUnique()
            .HasDatabaseName("UQ_ProjectSkills_ProjectID_SkillID");

        // Properties
        builder.Property(prs => prs.IsRequired)
            .HasDefaultValue(true);

        builder.Property(prs => prs.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(prs => prs.ProjectID)
            .HasDatabaseName("IDX_ProjectSkills_ProjectID");

        builder.HasIndex(prs => prs.SkillID)
            .HasDatabaseName("IDX_ProjectSkills_SkillID");

        // Relationships
        builder.HasOne(prs => prs.Project)
            .WithMany(p => p.ProjectRequiredSkills)
            .HasForeignKey(prs => prs.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(prs => prs.Skill)
            .WithMany(s => s.ProjectRequiredSkills)
            .HasForeignKey(prs => prs.SkillID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
