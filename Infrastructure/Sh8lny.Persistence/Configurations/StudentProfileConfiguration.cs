using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
               .ValueGeneratedOnAdd();

        builder.Property(sp => sp.University)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(sp => sp.Major)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(sp => sp.GraduationDay)
               .IsRequired()
               .HasColumnType("date");

  
        builder.Property(sp => sp.TrainingDays)
               .HasMaxLength(500)
               .HasComment("JSON array of training days");

        builder.ToTable("StudentProfiles");
    }
}
