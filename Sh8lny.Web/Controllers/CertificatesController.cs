using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for certificate operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    /// <summary>
    /// Gets all certificates for the authenticated student.
    /// </summary>
    [HttpGet("my-certificates")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCertificates()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user token.");
        }

        var result = await _certificateService.GetMyCertificatesAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Verifies and retrieves a certificate by its unique identifier.
    /// This endpoint is publicly accessible for certificate verification.
    /// </summary>
    [HttpGet("verify/{uniqueId}")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCertificate(string uniqueId)
    {
        var result = await _certificateService.GetCertificateByIdentifierAsync(uniqueId);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Manually generates a certificate for a completed application.
    /// (Usually called automatically when a job is completed)
    /// </summary>
    [HttpPost("generate/{applicationId}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> GenerateCertificate(int applicationId)
    {
        var result = await _certificateService.GenerateCertificateAsync(applicationId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
