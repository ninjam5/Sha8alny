using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
{
    public void Configure(EntityTypeBuilder<StudentSkill> builder)
    {
        builder.HasKey(ss => new { ss.StudentId, ss.SkillId });

        builder.HasOne(ss => ss.StudentProfile)
               .WithMany(sp => sp.StudentSkills)
               .HasForeignKey(ss => ss.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ss => ss.Skill)
               .WithMany(s => s.StudentSkills)
               .HasForeignKey(ss => ss.SkillId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("StudentSkills");
    }
}
