using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Review entity
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // Primary key
        builder.HasKey(r => r.Id);

        // Rating property with check constraint (1-5 stars)
        builder.Property(r => r.Rating).IsRequired();
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        // Comment property with max length
        builder.Property(r => r.Comment).HasMaxLength(1000);

        // Timestamp with database default
        builder.Property(r => r.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Performance indexes
        builder.HasIndex(r => r.Rating);
        builder.HasIndex(r => r.Created_At);

        // CompletedOpportunity relationship (RESTRICT for data integrity)
        builder.HasOne(r => r.CompletedOpportunity)
               .WithMany(co => co.Reviews)
               .HasForeignKey(r => r.Completed_id)
               .OnDelete(DeleteBehavior.Restrict);

        // Reviewer relationship (RESTRICT to preserve user data)
        builder.HasOne(r => r.Reviewer)
               .WithMany(u => u.ReviewsWritten)
               .HasForeignKey(r => r.UIdReviewer)
               .OnDelete(DeleteBehavior.Restrict);

        // Review target relationship (RESTRICT to preserve user data)
        builder.HasOne(r => r.Target)
               .WithMany(u => u.ReviewsReceived)
               .HasForeignKey(r => r.UIdTarget)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Reviews");
    }
}
