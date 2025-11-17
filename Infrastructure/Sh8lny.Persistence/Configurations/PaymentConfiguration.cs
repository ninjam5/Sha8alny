using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table mapping
        builder.ToTable("Payments");

        // Primary key
        builder.HasKey(p => p.PaymentID);

        // Properties
        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.TransactionID)
            .HasMaxLength(200);

        builder.Property(p => p.PaymentGateway)
            .HasMaxLength(100);

        builder.Property(p => p.PaymentReference)
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(p => p.ProjectID)
            .HasDatabaseName("IDX_Payments_ProjectID");

        builder.HasIndex(p => p.StudentID)
            .HasDatabaseName("IDX_Payments_StudentID");

        builder.HasIndex(p => p.CompanyID)
            .HasDatabaseName("IDX_Payments_CompanyID");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IDX_Payments_Status");

        builder.HasIndex(p => p.TransactionID)
            .HasDatabaseName("IDX_Payments_TransactionID");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IDX_Payments_CreatedAt");

        // Relationships
        builder.HasOne(p => p.Project)
            .WithMany(proj => proj.Payments)
            .HasForeignKey(p => p.ProjectID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Student)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Company)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CompanyID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
