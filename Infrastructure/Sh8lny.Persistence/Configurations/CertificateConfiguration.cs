using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Certificate entity
/// </summary>
public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        // Table mapping
        builder.ToTable("Certificates");

        // Primary key
        builder.HasKey(c => c.CertificateID);

        // Properties
        builder.Property(c => c.CertificateNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CertificateTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.CertificateURL)
            .HasMaxLength(500);

        builder.Property(c => c.IssuedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(c => c.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IDX_Certificates_CertificateNumber");

        builder.HasIndex(c => c.StudentID)
            .HasDatabaseName("IDX_Certificates_StudentID");

        builder.HasIndex(c => c.ProjectID)
            .HasDatabaseName("IDX_Certificates_ProjectID");

        builder.HasIndex(c => c.CompanyID)
            .HasDatabaseName("IDX_Certificates_CompanyID");
    }
}
