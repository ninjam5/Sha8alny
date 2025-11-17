using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.UserSettings;
using Sh8lny.Application.Interfaces;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly ILogger<UserSettingsController> _logger;

    public UserSettingsController(IUserSettingsService userSettingsService, ILogger<UserSettingsController> logger)
    {
        _userSettingsService = userSettingsService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMySettings()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userSettingsService.GetUserSettingsAsync(userId);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMySettings([FromBody] UpdateUserSettingsDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        dto.UserID = userId; // Override DTO userId with authenticated user
        var result = await _userSettingsService.UpdateUserSettingsAsync(dto);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserSettings(int userId)
    {
        var result = await _userSettingsService.GetUserSettingsAsync(userId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserSettings([FromBody] UpdateUserSettingsDto dto)
    {
        var result = await _userSettingsService.UpdateUserSettingsAsync(dto);
        return Ok(result);
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] NotificationPreferencesDto dto)
    {
        var result = await _userSettingsService.UpdateNotificationPreferencesAsync(dto);
        return Ok(result);
    }

    [HttpPut("privacy")]
    public async Task<IActionResult> UpdatePrivacySettings([FromBody] PrivacySettingsDto dto)
    {
        var result = await _userSettingsService.UpdatePrivacySettingsAsync(dto);
        return Ok(result);
    }

    [HttpPost("{userId}/default")]
    public async Task<IActionResult> CreateDefaultSettings(int userId)
    {
        var result = await _userSettingsService.CreateDefaultSettingsAsync(userId);
        return CreatedAtAction(nameof(GetUserSettings), new { userId }, result);
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserSettings(int userId)
    {
        await _userSettingsService.DeleteUserSettingsAsync(userId);
        return NoContent();
    }
}
