using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for CompanyReview entity
/// </summary>
public class CompanyReviewConfiguration : IEntityTypeConfiguration<CompanyReview>
{
    public void Configure(EntityTypeBuilder<CompanyReview> builder)
    {
        // Table mapping
        builder.ToTable("CompanyReviews");

        // Primary key
        builder.HasKey(cr => cr.ReviewID);

        // Properties
        builder.Property(cr => cr.Rating)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.ReviewTitle)
            .HasMaxLength(200);

        builder.Property(cr => cr.ReviewText)
            .HasMaxLength(2000);

        builder.Property(cr => cr.WorkEnvironmentRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.LearningOpportunityRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.MentorshipRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.CompensationRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.CommunicationRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(cr => cr.WouldRecommend)
            .HasDefaultValue(true);

        builder.Property(cr => cr.Pros)
            .HasMaxLength(1000);

        builder.Property(cr => cr.Cons)
            .HasMaxLength(1000);

        builder.Property(cr => cr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(cr => cr.IsVerified)
            .HasDefaultValue(false);

        builder.Property(cr => cr.IsAnonymous)
            .HasDefaultValue(false);

        builder.Property(cr => cr.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(cr => cr.CompanyResponse)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(cr => cr.CompanyID)
            .HasDatabaseName("IDX_CompanyReviews_CompanyID");

        builder.HasIndex(cr => cr.StudentID)
            .HasDatabaseName("IDX_CompanyReviews_StudentID");

        builder.HasIndex(cr => cr.CompletedOpportunityID)
            .HasDatabaseName("IDX_CompanyReviews_CompletedOpportunityID");

        builder.HasIndex(cr => cr.Status)
            .HasDatabaseName("IDX_CompanyReviews_Status");

        builder.HasIndex(cr => cr.Rating)
            .HasDatabaseName("IDX_CompanyReviews_Rating");

        builder.HasIndex(cr => cr.CreatedAt)
            .HasDatabaseName("IDX_CompanyReviews_CreatedAt");

        // Unique constraint: one review per student per completed opportunity
        builder.HasIndex(cr => new { cr.StudentID, cr.CompletedOpportunityID })
            .IsUnique()
            .HasFilter("[CompletedOpportunityID] IS NOT NULL")
            .HasDatabaseName("UQ_CompanyReviews_StudentID_CompletedOpportunityID");

        // Relationships
        builder.HasOne(cr => cr.Company)
            .WithMany(c => c.Reviews)
            .HasForeignKey(cr => cr.CompanyID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Student)
            .WithMany(s => s.CompanyReviews)
            .HasForeignKey(cr => cr.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.CompletedOpportunity)
            .WithOne(co => co.CompanyReview)
            .HasForeignKey<CompanyReview>(cr => cr.CompletedOpportunityID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
