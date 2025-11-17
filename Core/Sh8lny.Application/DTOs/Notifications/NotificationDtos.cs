using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.Notifications;

#region Notification DTOs

/// <summary>
/// DTO for creating a new notification
/// </summary>
public class CreateNotificationDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    [Required(ErrorMessage = "Notification type is required")]
    public int NotificationType { get; set; } // 0-6: Application, Message, Project, Deadline, Acceptance, Rejection, System

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
    public string Message { get; set; } = string.Empty;

    public int? RelatedProjectID { get; set; }
    public int? RelatedApplicationID { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? ActionURL { get; set; }
}

/// <summary>
/// DTO for notification details
/// </summary>
public class NotificationDto
{
    public int NotificationID { get; set; }
    public int UserID { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? RelatedProjectID { get; set; }
    public int? RelatedApplicationID { get; set; }
    public string? ActionURL { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// DTO for marking notifications as read
/// </summary>
public class MarkNotificationsAsReadDto
{
    [Required(ErrorMessage = "At least one notification ID is required")]
    [MinLength(1, ErrorMessage = "At least one notification ID is required")]
    public List<int> NotificationIDs { get; set; } = new();
}

/// <summary>
/// DTO for notification statistics
/// </summary>
public class NotificationStatsDto
{
    public int TotalNotifications { get; set; }
    public int UnreadCount { get; set; }
    public int ApplicationNotifications { get; set; }
    public int MessageNotifications { get; set; }
    public int ProjectNotifications { get; set; }
}

#endregion
