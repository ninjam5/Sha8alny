using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for DashboardMetric entity
/// </summary>
public class DashboardMetricConfiguration : IEntityTypeConfiguration<DashboardMetric>
{
    public void Configure(EntityTypeBuilder<DashboardMetric> builder)
    {
        // Table mapping
        builder.ToTable("DashboardMetrics");

        // Primary key
        builder.HasKey(dm => dm.MetricID);

        // Properties
        builder.Property(dm => dm.TotalStudents)
            .HasDefaultValue(0);

        builder.Property(dm => dm.TotalProjects)
            .HasDefaultValue(0);

        builder.Property(dm => dm.CompletedProjects)
            .HasDefaultValue(0);

        builder.Property(dm => dm.AvailableOpportunities)
            .HasDefaultValue(0);

        builder.Property(dm => dm.NewApplicants)
            .HasDefaultValue(0);

        builder.Property(dm => dm.ActivityIncreasePercent)
            .HasColumnType("decimal(5,2)");

        builder.Property(dm => dm.MetricDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(dm => dm.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(dm => dm.MetricDate)
            .HasDatabaseName("IDX_DashboardMetrics_MetricDate");

        // Note: CompanyID and UniversityID removed from model but kept as FKs
    }
}
