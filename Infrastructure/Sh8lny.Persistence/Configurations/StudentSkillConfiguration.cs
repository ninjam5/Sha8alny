using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for StudentSkill junction table
public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
{
    public void Configure(EntityTypeBuilder<StudentSkill> builder)
    {
        // Composite primary key
        builder.HasKey(ss => new { ss.StudentId, ss.SkillId });

        // Many-to-many relationship: StudentProfile side (CASCADE delete)
        builder.HasOne(ss => ss.StudentProfile)
               .WithMany(sp => sp.StudentSkills)
               .HasForeignKey(ss => ss.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many relationship: Skill side (CASCADE delete)
        builder.HasOne(ss => ss.Skill)
               .WithMany(s => s.StudentSkills)
               .HasForeignKey(ss => ss.SkillId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("StudentSkills");
    }
}
