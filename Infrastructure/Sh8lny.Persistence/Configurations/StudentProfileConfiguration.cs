using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for StudentProfile entity
public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        // Primary key
        builder.HasKey(sp => sp.Id);

        // Required properties with max lengths
        builder.Property(sp => sp.University).IsRequired().HasMaxLength(200);
        builder.Property(sp => sp.Major).IsRequired().HasMaxLength(150);
        builder.Property(sp => sp.GraduationYear).IsRequired().HasColumnType("date");

        // Applications relationship (RESTRICT to prevent cascade path cycles)
        builder.HasMany(s => s.Applications)
               .WithOne(a => a.StudentProfile)
               .HasForeignKey(a => a.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        // StudentSkills relationship (CASCADE for junction table cleanup)
        builder.HasMany(sp => sp.StudentSkills)
               .WithOne(ss => ss.StudentProfile)
               .HasForeignKey(ss => ss.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // CompletedOpportunities relationship (RESTRICT for data integrity)
        builder.HasMany(sp => sp.CompletedOpportunities)
               .WithOne(co => co.StudentProfile)
               .HasForeignKey(co => co.StudentProfileId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("StudentProfiles");
    }
}
