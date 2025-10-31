using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
               .ValueGeneratedOnAdd();

        builder.Property(cp => cp.WebSite)
               .HasMaxLength(255);

        builder.Property(cp => cp.Industry)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(cp => cp.Location)
               .IsRequired()
               .HasMaxLength(200);

        builder.ToTable("CompanyProfiles");
    }
}
