using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
               .IsUnique();

        builder.Property(c => c.Icon_URL)
               .HasMaxLength(500);

        builder.Property(c => c.Created_At)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.ToTable("Categories");
    }
}
