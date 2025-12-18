using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Notifications;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for notification operations.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gets all notifications for the current user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Service response containing the list of notifications.</returns>
    Task<ServiceResponse<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(int userId);

    /// <summary>
    /// Gets unread notifications count for the current user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Service response containing the unread count.</returns>
    Task<ServiceResponse<int>> GetUnreadCountAsync(int userId);

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="notificationId">The notification ID.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> MarkAsReadAsync(int userId, int notificationId);

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> MarkAllAsReadAsync(int userId);

    /// <summary>
    /// Sends a real-time notification to a user (via SignalR).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="notification">The notification DTO.</param>
    /// <returns>Task representing the async operation.</returns>
    Task SendRealTimeNotificationAsync(int userId, NotificationDto notification);
}
