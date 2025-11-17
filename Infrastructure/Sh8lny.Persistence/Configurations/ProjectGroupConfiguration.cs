using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for ProjectGroup entity
/// </summary>
public class ProjectGroupConfiguration : IEntityTypeConfiguration<ProjectGroup>
{
    public void Configure(EntityTypeBuilder<ProjectGroup> builder)
    {
        // Table mapping
        builder.ToTable("ProjectGroups");

        // Primary key
        builder.HasKey(pg => pg.GroupID);

        // Properties
        builder.Property(pg => pg.GroupName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pg => pg.Description)
            .HasMaxLength(1000);

        builder.Property(pg => pg.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(pg => pg.ProjectID)
            .HasDatabaseName("IDX_ProjectGroups_ProjectID");

        // Relationships
        builder.HasOne(pg => pg.Project)
            .WithMany(p => p.ProjectGroups)
            .HasForeignKey(pg => pg.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pg => pg.GroupMembers)
            .WithOne(gm => gm.ProjectGroup)
            .HasForeignKey(gm => gm.GroupID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
