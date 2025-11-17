using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.CompanyID);

            builder.Property(c => c.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.ContactEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Additional properties
            builder.Property(c => c.CompanyLogo)
                .HasMaxLength(500);

            builder.Property(c => c.ContactPhone)
                .HasMaxLength(20);

            builder.Property(c => c.Website)
                .HasMaxLength(500);

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.State)
                .HasMaxLength(100);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.Industry)
                .HasMaxLength(100);

            // Rating & Reviews
            builder.Property(c => c.AverageRating)
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.TotalReviews)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(c => c.UserID)
                .HasDatabaseName("IDX_Companies_UserID");

            builder.HasIndex(c => c.AverageRating)
                .HasDatabaseName("IDX_Companies_AverageRating");

            // Relationships
            builder.HasMany(c => c.Projects)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.IssuedCertificates)
                .WithOne(cert => cert.Company)
                .HasForeignKey(cert => cert.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);

            // DashboardMetrics relationship removed from model
        }
    }
}
