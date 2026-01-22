using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Shared.DTOs.Payments;

/// <summary>
/// DTO for processing a payment from Company to Student.
/// </summary>
public class ProcessPaymentDto
{
    /// <summary>
    /// The application ID to process payment for.
    /// </summary>
    [Required]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Payment method to use (e.g., "Visa", "MasterCard", "Wallet", "BankTransfer").
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;
}
