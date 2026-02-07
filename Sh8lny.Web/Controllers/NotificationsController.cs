using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Notifications;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for notification management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Gets all notifications for the current user.
    /// </summary>
    /// <returns>List of notifications.</returns>
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<IEnumerable<NotificationDto>>>> GetMyNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<IEnumerable<NotificationDto>>.Failure("Invalid or missing user token."));
        }

        var result = await _notificationService.GetUserNotificationsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets the count of unread notifications for the current user.
    /// </summary>
    /// <returns>Unread notification count.</returns>
    [HttpGet("unread-count")]
    public async Task<ActionResult<ServiceResponse<int>>> GetUnreadCount()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<int>.Failure("Invalid or missing user token."));
        }

        var result = await _notificationService.GetUnreadCountAsync(userId.Value);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="id">The notification ID.</param>
    /// <returns>Success or failure response.</returns>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<ServiceResponse<bool>>> MarkAsRead(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _notificationService.MarkAsReadAsync(userId.Value, id);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    /// <returns>Success or failure response.</returns>
    [HttpPut("read-all")]
    public async Task<ActionResult<ServiceResponse<bool>>> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _notificationService.MarkAllAsReadAsync(userId.Value);

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
