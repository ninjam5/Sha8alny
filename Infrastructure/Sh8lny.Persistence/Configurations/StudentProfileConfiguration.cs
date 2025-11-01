using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.University)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(sp => sp.Major)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(sp => sp.GraduationYear)
               .IsRequired()
               .HasColumnType("date");

        builder.HasMany(s => s.Applications)
                   .WithOne(a => a.StudentProfile)
                   .HasForeignKey(a => a.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sp => sp.StudentSkills)
                   .WithOne(ss => ss.StudentProfile)
                   .HasForeignKey(ss => ss.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.CompletedOpportunities)
                   .WithOne(co => co.StudentProfile)
                   .HasForeignKey(co => co.StudentProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable("StudentProfiles");
    }
}
