using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Payments;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for payment processing operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes a payment from a Company to a Student for a completed job.
    /// </summary>
    /// <param name="companyUserId">The company's user ID (payer).</param>
    /// <param name="dto">The payment processing details.</param>
    /// <returns>Service response containing the payment receipt.</returns>
    Task<ServiceResponse<PaymentReceiptDto>> ProcessPaymentAsync(int companyUserId, ProcessPaymentDto dto);
}
