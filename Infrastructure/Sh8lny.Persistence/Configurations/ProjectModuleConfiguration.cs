using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent configuration for ProjectModule entity
/// </summary>
public class ProjectModuleConfiguration : IEntityTypeConfiguration<ProjectModule>
{
    public void Configure(EntityTypeBuilder<ProjectModule> builder)
    {
        builder.ToTable("ProjectModules");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.EstimatedDuration)
            .HasMaxLength(100);

        builder.Property(m => m.OrderIndex)
            .IsRequired();

        builder.HasIndex(m => new { m.ProjectId, m.OrderIndex })
            .HasDatabaseName("IDX_ProjectModules_Project_Order");

        builder.HasOne(m => m.Project)
            .WithMany(p => p.Modules)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
