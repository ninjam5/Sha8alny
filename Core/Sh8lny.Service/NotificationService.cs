using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Notifications;

namespace Sh8lny.Service;

/// <summary>
/// Service for notification operations.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public NotificationService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    /// <inheritdoc />
    public async Task SendRealTimeNotificationAsync(int userId, NotificationDto notification)
    {
        await _notifier.SendNotificationAsync(userId, notification);
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(int userId)
    {
        try
        {
            var notifications = await _unitOfWork.Notifications
                .FindAsync(n => n.UserID == userId);

            var dtos = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.NotificationID,
                    Title = n.Title,
                    Message = n.Message,
                    NotificationType = n.NotificationType.ToString(),
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ReadAt = n.ReadAt,
                    RelatedProjectId = n.RelatedProjectID,
                    RelatedApplicationId = n.RelatedApplicationID,
                    ActionUrl = n.ActionURL
                })
                .ToList();

            return ServiceResponse<IEnumerable<NotificationDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<NotificationDto>>.Failure(
                "An error occurred while retrieving notifications.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<int>> GetUnreadCountAsync(int userId)
    {
        try
        {
            var unreadNotifications = await _unitOfWork.Notifications
                .FindAsync(n => n.UserID == userId && !n.IsRead);

            var count = unreadNotifications.Count();

            return ServiceResponse<int>.Success(count);
        }
        catch (Exception ex)
        {
            return ServiceResponse<int>.Failure(
                "An error occurred while counting unread notifications.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> MarkAsReadAsync(int userId, int notificationId)
    {
        try
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);

            if (notification is null)
            {
                return ServiceResponse<bool>.Failure("Notification not found.");
            }

            // Security check: ensure user owns this notification
            if (notification.UserID != userId)
            {
                return ServiceResponse<bool>.Failure("You do not have permission to modify this notification.");
            }

            if (notification.IsRead)
            {
                return ServiceResponse<bool>.Success(true, "Notification is already marked as read.");
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveAsync();

            return ServiceResponse<bool>.Success(true, "Notification marked as read.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while marking notification as read.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var unreadNotifications = await _unitOfWork.Notifications
                .FindAsync(n => n.UserID == userId && !n.IsRead);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _unitOfWork.Notifications.Update(notification);
            }

            await _unitOfWork.SaveAsync();

            return ServiceResponse<bool>.Success(true, "All notifications marked as read.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while marking all notifications as read.",
                new List<string> { ex.Message });
        }
    }
}
