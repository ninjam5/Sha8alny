namespace Sh8lny.Domain.Entities;

/// <summary>
/// Payment transaction entity for freelance projects and paid opportunities
/// </summary>
public class Payment
{
    // Primary key
    public int PaymentID { get; set; }

    // Foreign keys
    public int ProjectID { get; set; }
    public int StudentID { get; set; }
    public int? CompanyID { get; set; }

    // Payment details
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    // Transaction info
    public string? TransactionID { get; set; }
    public string? PaymentGateway { get; set; }
    public string? PaymentReference { get; set; }

    // Description
    public string? Description { get; set; }
    public string? Notes { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public Company? Company { get; set; }
}

/// <summary>
/// Payment status enumeration
/// </summary>
public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded,
    Cancelled
}

/// <summary>
/// Payment method enumeration
/// </summary>
public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    BankTransfer,
    PayPal,
    Stripe,
    Cash,
    Wallet,
    Other
}
