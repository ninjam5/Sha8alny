using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
{
    public void Configure(EntityTypeBuilder<StudentSkill> builder)
    {
        builder.HasKey(ss => ss.Id);

        builder.Property(ss => ss.Id)
               .ValueGeneratedOnAdd();

        builder.Property(ss => ss.SkillName)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(ss => ss.SkillName);

        builder.ToTable("StudentSkills");
    }
}
