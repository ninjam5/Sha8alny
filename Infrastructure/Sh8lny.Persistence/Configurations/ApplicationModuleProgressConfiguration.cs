using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent configuration for ApplicationModuleProgress entity
/// </summary>
public class ApplicationModuleProgressConfiguration : IEntityTypeConfiguration<ApplicationModuleProgress>
{
    public void Configure(EntityTypeBuilder<ApplicationModuleProgress> builder)
    {
        builder.ToTable("ApplicationModuleProgress");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IsCompleted)
            .HasDefaultValue(false);

        builder.Property(p => p.CompletedAt)
            .IsRequired(false);

        builder.HasIndex(p => new { p.ApplicationId, p.ProjectModuleId })
            .IsUnique()
            .HasDatabaseName("UQ_ApplicationModuleProgress");

        builder.HasOne(p => p.Application)
            .WithMany(a => a.ModuleProgress)
            .HasForeignKey(p => p.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.ProjectModule)
            .WithMany(m => m.ModuleProgress)
            .HasForeignKey(p => p.ProjectModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
