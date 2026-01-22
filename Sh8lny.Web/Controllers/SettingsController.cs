using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Settings;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for user settings operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IUserSettingsService _settingsService;

    public SettingsController(IUserSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Gets settings for the current authenticated user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user token.");
        }

        var result = await _settingsService.GetSettingsAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Updates settings for the current authenticated user.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user token.");
        }

        var result = await _settingsService.UpdateSettingsAsync(userId, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
