using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for StudentReview entity
/// </summary>
public class StudentReviewConfiguration : IEntityTypeConfiguration<StudentReview>
{
    public void Configure(EntityTypeBuilder<StudentReview> builder)
    {
        // Table mapping
        builder.ToTable("StudentReviews");

        // Primary key
        builder.HasKey(sr => sr.ReviewID);

        // Properties
        builder.Property(sr => sr.Rating)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.ReviewTitle)
            .HasMaxLength(200);

        builder.Property(sr => sr.ReviewText)
            .HasMaxLength(2000);

        builder.Property(sr => sr.TechnicalSkillsRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.CommunicationRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.ProfessionalismRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.TimeManagementRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.TeamworkRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.ProblemSolvingRating)
            .HasColumnType("decimal(3,2)");

        builder.Property(sr => sr.WouldHireAgain)
            .HasDefaultValue(true);

        builder.Property(sr => sr.Strengths)
            .HasMaxLength(1000);

        builder.Property(sr => sr.AreasForImprovement)
            .HasMaxLength(1000);

        builder.Property(sr => sr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sr => sr.IsVerified)
            .HasDefaultValue(false);

        builder.Property(sr => sr.IsPublic)
            .HasDefaultValue(true);

        builder.Property(sr => sr.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(sr => sr.StudentResponse)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(sr => sr.StudentID)
            .HasDatabaseName("IDX_StudentReviews_StudentID");

        builder.HasIndex(sr => sr.CompanyID)
            .HasDatabaseName("IDX_StudentReviews_CompanyID");

        builder.HasIndex(sr => sr.CompletedOpportunityID)
            .HasDatabaseName("IDX_StudentReviews_CompletedOpportunityID");

        builder.HasIndex(sr => sr.Status)
            .HasDatabaseName("IDX_StudentReviews_Status");

        builder.HasIndex(sr => sr.Rating)
            .HasDatabaseName("IDX_StudentReviews_Rating");

        builder.HasIndex(sr => sr.CreatedAt)
            .HasDatabaseName("IDX_StudentReviews_CreatedAt");

        // Unique constraint: one review per company per completed opportunity
        builder.HasIndex(sr => new { sr.CompanyID, sr.CompletedOpportunityID })
            .IsUnique()
            .HasFilter("[CompletedOpportunityID] IS NOT NULL")
            .HasDatabaseName("UQ_StudentReviews_CompanyID_CompletedOpportunityID");

        // Relationships
        builder.HasOne(sr => sr.Student)
            .WithMany(s => s.ReceivedReviews)
            .HasForeignKey(sr => sr.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Company)
            .WithMany(c => c.StudentReviews)
            .HasForeignKey(sr => sr.CompanyID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.CompletedOpportunity)
            .WithOne(co => co.StudentReview)
            .HasForeignKey<StudentReview>(sr => sr.CompletedOpportunityID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
