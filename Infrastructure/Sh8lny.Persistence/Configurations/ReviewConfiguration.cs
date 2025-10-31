using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
               .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        builder.Property(r => r.Comment)
               .HasMaxLength(1000);

        builder.Property(r => r.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(r => r.Rating);

        builder.HasIndex(r => r.Created_At);

        builder.HasOne(r => r.CompletedOpportunity)
                   .WithMany(co => co.Reviews)
                   .HasForeignKey(r => r.Completed_id)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Reviewer)
                   .WithMany(u => u.ReviewsWritten)
                   .HasForeignKey(r => r.UIdReviewer)
                   .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Target)
               .WithMany(u => u.ReviewsReceived)
               .HasForeignKey(r => r.UIdTarget)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Reviews");
    }
}
