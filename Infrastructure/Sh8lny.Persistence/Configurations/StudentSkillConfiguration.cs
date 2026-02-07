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
    public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
    {
        public void Configure(EntityTypeBuilder<StudentSkill> builder)
        {
            // Table mapping
            builder.ToTable("StudentSkills");

            // Primary key
            builder.HasKey(ss => ss.StudentSkillID);

            // Unique constraint
            builder.HasIndex(ss => new { ss.StudentID, ss.SkillID })
                .IsUnique()
                .HasDatabaseName("UQ_StudentSkills_StudentID_SkillID");

            // Properties
            builder.Property(ss => ss.ProficiencyLevel)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(ss => ss.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Indexes
            builder.HasIndex(ss => ss.StudentID)
                .HasDatabaseName("IDX_StudentSkills_StudentID");

            builder.HasIndex(ss => ss.SkillID)
                .HasDatabaseName("IDX_StudentSkills_SkillID");

            // Relationships
            builder.HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSkills)
                .HasForeignKey(ss => ss.StudentID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ss => ss.Skill)
                .WithMany(s => s.StudentSkills)
                .HasForeignKey(ss => ss.SkillID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
