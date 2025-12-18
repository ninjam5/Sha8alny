using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.CompanyProfile;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for company profile management.
/// </summary>
[ApiController]
[Route("api/company")]
[Authorize(Roles = "Company")]
public class CompanyProfileController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyProfileController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Creates or updates the company profile for the authenticated user.
    /// </summary>
    /// <param name="dto">The company profile data.</param>
    /// <returns>The company ID.</returns>
    [HttpPost("profile")]
    public async Task<ActionResult<ServiceResponse<int>>> CreateOrUpdateProfile([FromBody] CreateCompanyProfileDto dto)
    {
        // Extract UserId from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ServiceResponse<int>.Failure("Invalid or missing user token."));
        }

        var result = await _companyService.CreateOrUpdateProfileAsync(userId, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets the company profile for the authenticated user.
    /// </summary>
    /// <returns>The company profile.</returns>
    [HttpGet("profile")]
    public async Task<ActionResult<ServiceResponse<CompanyDto>>> GetProfile()
    {
        // Extract UserId from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ServiceResponse<CompanyDto>.Failure("Invalid or missing user token."));
        }

        var result = await _companyService.GetProfileAsync(userId);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
