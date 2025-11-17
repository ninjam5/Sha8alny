using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Department entity
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Table mapping
        builder.ToTable("Departments");

        // Primary key
        builder.HasKey(d => d.DepartmentID);

        // Properties
        builder.Property(d => d.DepartmentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(d => d.UniversityID)
            .HasDatabaseName("IDX_Departments_UniversityID");

        builder.HasIndex(d => d.IsActive)
            .HasDatabaseName("IDX_Departments_IsActive");

        // Relationships
        // University relationship removed from model, keeping FK only
    }
}
