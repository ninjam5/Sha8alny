using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Payments;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for payment operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Processes a payment from a Company to a Student for a completed job.
    /// </summary>
    /// <param name="dto">The payment processing details.</param>
    /// <returns>Payment receipt on success.</returns>
    [HttpPost("pay")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<PaymentReceiptDto>>> ProcessPayment([FromBody] ProcessPaymentDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<PaymentReceiptDto>.Failure("Invalid or missing user token."));
        }

        var result = await _paymentService.ProcessPaymentAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Extracts the current user ID from JWT claims.
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }
        return userId;
    }
}
