using Sh8lny.Application.DTOs.Notifications;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Notifications;

/// <summary>
/// Service for notification management
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserSettingsService _userSettingsService;

    public NotificationService(IUnitOfWork unitOfWork, IUserSettingsService userSettingsService)
    {
        _unitOfWork = unitOfWork;
        _userSettingsService = userSettingsService;
    }

    public async Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto dto)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserID);

        // Validate notification type
        if (!Enum.IsDefined(typeof(NotificationType), dto.NotificationType))
            throw new ValidationException("Invalid notification type");

        // Get user's notification preferences
        var settings = await _userSettingsService.GetUserSettingsAsync(dto.UserID);

        // Check if user has enabled notifications for this type
        bool allowNotification = true;
        var notificationType = (NotificationType)dto.NotificationType;

        switch (notificationType)
        {
            case NotificationType.Application:
            case NotificationType.Acceptance:
            case NotificationType.Rejection:
                // Application-related notifications
                if (!settings.ApplicationNotifications)
                    allowNotification = false;
                break;

            case NotificationType.Message:
                // Message notifications
                if (!settings.MessageNotifications)
                    allowNotification = false;
                break;

            case NotificationType.Project:
            case NotificationType.Deadline:
            case NotificationType.System:
            case NotificationType.Certificate:
                // General push notifications (projects, deadlines, system, certificates)
                if (!settings.PushNotifications)
                    allowNotification = false;
                break;

            default:
                // For any unknown types, respect push notification setting
                if (!settings.PushNotifications)
                    allowNotification = false;
                break;
        }

        // If user has disabled this notification type, don't create it
        if (!allowNotification)
        {
            return null;
        }

        // Create notification entity
        var notification = new Notification
        {
            UserID = dto.UserID,
            NotificationType = notificationType,
            Title = dto.Title,
            Message = dto.Message,
            RelatedProjectID = dto.RelatedProjectID,
            RelatedApplicationID = dto.RelatedApplicationID,
            ActionURL = dto.ActionURL,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return MapToNotificationDto(notification);
    }

    public async Task<NotificationDto> GetNotificationByIdAsync(int notificationId, int userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
            throw new NotFoundException(nameof(Notification), notificationId);

        // Verify ownership
        if (notification.UserID != userId)
            throw new UnauthorizedException("You are not authorized to access this notification");

        return MapToNotificationDto(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 50)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        // Get notifications for user (already ordered by CreatedAt descending in repository)
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
        
        // Apply pagination
        var pagedNotifications = notifications
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return pagedNotifications.Select(MapToNotificationDto);
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var notifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(userId);
        return notifications.Select(MapToNotificationDto);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
            throw new NotFoundException(nameof(Notification), notificationId);

        // Verify ownership
        if (notification.UserID != userId)
            throw new UnauthorizedException("You are not authorized to modify this notification");

        // Mark as read
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkMultipleAsReadAsync(MarkNotificationsAsReadDto dto, int userId)
    {
        if (dto.NotificationIDs == null || !dto.NotificationIDs.Any())
            throw new ValidationException("No notification IDs provided");

        foreach (var notificationId in dto.NotificationIDs)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
                continue; // Skip non-existent notifications

            // Verify ownership
            if (notification.UserID != userId)
                throw new UnauthorizedException($"You are not authorized to modify notification {notificationId}");

            // Mark as read
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        // Use repository method to mark all as read
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
            throw new NotFoundException(nameof(Notification), notificationId);

        // Verify ownership
        if (notification.UserID != userId)
            throw new UnauthorizedException("You are not authorized to delete this notification");

        await _unitOfWork.Notifications.DeleteAsync(notification);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<NotificationStatsDto> GetNotificationStatsAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
        var notificationsList = notifications.ToList();

        return new NotificationStatsDto
        {
            TotalNotifications = notificationsList.Count,
            UnreadCount = notificationsList.Count(n => !n.IsRead),
            ApplicationNotifications = notificationsList.Count(n => n.NotificationType == NotificationType.Application),
            MessageNotifications = notificationsList.Count(n => n.NotificationType == NotificationType.Message),
            ProjectNotifications = notificationsList.Count(n => n.NotificationType == NotificationType.Project)
        };
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var unreadNotifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(userId);
        return unreadNotifications.Count();
    }

    /// <summary>
    /// Maps Notification entity to DTO
    /// </summary>
    private NotificationDto MapToNotificationDto(Notification notification)
    {
        return new NotificationDto
        {
            NotificationID = notification.NotificationID,
            UserID = notification.UserID,
            NotificationType = notification.NotificationType.ToString(),
            Title = notification.Title,
            Message = notification.Message,
            RelatedProjectID = notification.RelatedProjectID,
            RelatedApplicationID = notification.RelatedApplicationID,
            ActionURL = notification.ActionURL,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt
        };
    }
}
