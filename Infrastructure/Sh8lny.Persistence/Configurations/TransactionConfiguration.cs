using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sh8lny.Domain.Models;

namespace Sh8lny.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the Transaction entity.
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        // Table name
        builder.ToTable("Transactions");

        // Primary key
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("TransactionID")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(t => t.ApplicationId)
            .IsRequired();

        builder.Property(t => t.PayerId)
            .IsRequired();

        builder.Property(t => t.PayeeId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.PaymentMethod)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.ReferenceId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Relationships
        builder.HasOne(t => t.Application)
            .WithMany()
            .HasForeignKey(t => t.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Payer)
            .WithMany()
            .HasForeignKey(t => t.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Payee)
            .WithMany()
            .HasForeignKey(t => t.PayeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.ApplicationId)
            .HasDatabaseName("IDX_Transactions_ApplicationId");

        builder.HasIndex(t => t.PayerId)
            .HasDatabaseName("IDX_Transactions_PayerId");

        builder.HasIndex(t => t.PayeeId)
            .HasDatabaseName("IDX_Transactions_PayeeId");

        builder.HasIndex(t => t.ReferenceId)
            .IsUnique()
            .HasDatabaseName("IDX_Transactions_ReferenceId");

        builder.HasIndex(t => t.TransactionDate)
            .HasDatabaseName("IDX_Transactions_TransactionDate");
    }
}
