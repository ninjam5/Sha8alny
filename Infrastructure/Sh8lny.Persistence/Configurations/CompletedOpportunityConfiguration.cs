using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for CompletedOpportunity entity
/// </summary>
public class CompletedOpportunityConfiguration : IEntityTypeConfiguration<CompletedOpportunity>
{
    public void Configure(EntityTypeBuilder<CompletedOpportunity> builder)
    {
        // Table mapping
        builder.ToTable("CompletedOpportunities");

        // Primary key
        builder.HasKey(co => co.CompletedOpportunityID);

        // Properties
        builder.Property(co => co.OpportunityTitle)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(co => co.Description)
            .HasMaxLength(2000);

        builder.Property(co => co.OpportunityType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(co => co.DurationInDays)
            .IsRequired();

        builder.Property(co => co.Rating)
            .HasColumnType("decimal(3,2)");

        builder.Property(co => co.StudentFeedback)
            .HasMaxLength(2000);

        builder.Property(co => co.CompanyFeedback)
            .HasMaxLength(2000);

        builder.Property(co => co.Achievements)
            .HasMaxLength(2000);

        builder.Property(co => co.SkillsGained)
            .HasMaxLength(1000);

        builder.Property(co => co.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(co => co.IsVerified)
            .HasDefaultValue(false);

        builder.Property(co => co.IsPaid)
            .HasDefaultValue(false);

        builder.Property(co => co.TotalPayment)
            .HasColumnType("decimal(18,2)");

        builder.Property(co => co.IsVisibleOnProfile)
            .HasDefaultValue(true);

        builder.Property(co => co.CompletedAt)
            .IsRequired();

        builder.Property(co => co.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(co => co.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        // Indexes
        builder.HasIndex(co => co.StudentID)
            .HasDatabaseName("IDX_CompletedOpportunities_StudentID");

        builder.HasIndex(co => co.ProjectID)
            .HasDatabaseName("IDX_CompletedOpportunities_ProjectID");

        builder.HasIndex(co => co.ApplicationID)
            .HasDatabaseName("IDX_CompletedOpportunities_ApplicationID");

        builder.HasIndex(co => co.CertificateID)
            .HasDatabaseName("IDX_CompletedOpportunities_CertificateID");

        builder.HasIndex(co => co.OpportunityType)
            .HasDatabaseName("IDX_CompletedOpportunities_OpportunityType");

        builder.HasIndex(co => co.Status)
            .HasDatabaseName("IDX_CompletedOpportunities_Status");

        builder.HasIndex(co => co.IsVerified)
            .HasDatabaseName("IDX_CompletedOpportunities_IsVerified");

        builder.HasIndex(co => co.CompletedAt)
            .HasDatabaseName("IDX_CompletedOpportunities_CompletedAt");

        // Relationships
        builder.HasOne(co => co.Student)
            .WithMany(s => s.CompletedOpportunities)
            .HasForeignKey(co => co.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(co => co.Project)
            .WithMany(p => p.CompletedOpportunities)
            .HasForeignKey(co => co.ProjectID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(co => co.Application)
            .WithOne(a => a.CompletedOpportunity)
            .HasForeignKey<CompletedOpportunity>(co => co.ApplicationID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(co => co.Certificate)
            .WithOne(c => c.CompletedOpportunity)
            .HasForeignKey<CompletedOpportunity>(co => co.CertificateID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(co => co.Verifier)
            .WithMany(u => u.VerifiedOpportunities)
            .HasForeignKey(co => co.VerifiedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
