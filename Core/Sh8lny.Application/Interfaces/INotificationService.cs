using Sh8lny.Application.DTOs.Notifications;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for notification management
/// </summary>
public interface INotificationService
{
    Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto dto);
    Task<NotificationDto> GetNotificationByIdAsync(int notificationId, int userId);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
    Task<bool> MarkAsReadAsync(int notificationId, int userId);
    Task<bool> MarkMultipleAsReadAsync(MarkNotificationsAsReadDto dto, int userId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    Task<NotificationStatsDto> GetNotificationStatsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
}
