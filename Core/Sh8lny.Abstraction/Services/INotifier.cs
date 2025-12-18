using Sh8lny.Shared.DTOs.Notifications;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for real-time notification delivery.
/// Implemented in the Web layer using SignalR.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Sends a real-time notification to a specific user.
    /// </summary>
    /// <param name="userId">The user ID to send the notification to.</param>
    /// <param name="notification">The notification data.</param>
    /// <returns>Task representing the async operation.</returns>
    Task SendNotificationAsync(int userId, NotificationDto notification);

    /// <summary>
    /// Sends a real-time notification to multiple users.
    /// </summary>
    /// <param name="userIds">The user IDs to send the notification to.</param>
    /// <param name="notification">The notification data.</param>
    /// <returns>Task representing the async operation.</returns>
    Task SendNotificationToManyAsync(IEnumerable<int> userIds, NotificationDto notification);
}
