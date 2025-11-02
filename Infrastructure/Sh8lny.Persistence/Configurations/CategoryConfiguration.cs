using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

// Fluent API configuration for Category entity
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);

        // Required properties with max lengths
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        // Timestamp with database default
        builder.Property(c => c.Created_At).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // One-to-many relationship with opportunities (RESTRICT to preserve category data)
        builder.HasMany(c => c.Opportunities)
               .WithOne(o => o.Category)
               .HasForeignKey(o => o.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Categories");
    }
}
