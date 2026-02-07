namespace Sh8lny.Shared.DTOs.Payments;

/// <summary>
/// DTO representing a payment receipt after successful transaction.
/// </summary>
public class PaymentReceiptDto
{
    /// <summary>
    /// The transaction ID.
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// Unique reference ID for this transaction.
    /// </summary>
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>
    /// The amount paid.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD", "EGP").
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Date and time of the transaction.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Payment method used.
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Name of the payer (Company).
    /// </summary>
    public string PayerName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the payee (Student).
    /// </summary>
    public string PayeeName { get; set; } = string.Empty;

    /// <summary>
    /// Project name for reference.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Success message.
    /// </summary>
    public string Message { get; set; } = "Payment Successful";
}
