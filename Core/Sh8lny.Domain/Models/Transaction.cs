namespace Sh8lny.Domain.Models;

/// <summary>
/// Represents a financial transaction between a Company (Payer) and a Student (Payee).
/// </summary>
public class Transaction
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The application this transaction is associated with.
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// The Company's User ID (Payer).
    /// </summary>
    public int PayerId { get; set; }

    /// <summary>
    /// The Student's User ID (Payee).
    /// </summary>
    public int PayeeId { get; set; }

    /// <summary>
    /// The transaction amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// When the transaction was processed.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Payment method used (e.g., "Credit Card", "Visa", "Wallet").
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Unique reference ID (GUID) for this transaction.
    /// </summary>
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>
    /// Transaction status.
    /// </summary>
    public TransactionStatus Status { get; set; }

    // Navigation properties
    public Application Application { get; set; } = null!;
    public User Payer { get; set; } = null!;
    public User Payee { get; set; } = null!;
}

/// <summary>
/// Transaction status enumeration.
/// </summary>
public enum TransactionStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
