namespace Sh8lny.Domain.Models;

/// <summary>
/// Notification entity for user alerts
/// </summary>
public class Notification
{
    // Primary key
    public int NotificationID { get; set; }

    // Foreign key
    public int UserID { get; set; }

    // Notification details
    public NotificationType NotificationType { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }

    // Related entities
    public int? RelatedProjectID { get; set; }
    public int? RelatedApplicationID { get; set; }

    // Action link
    public string? ActionURL { get; set; }

    // Status
    public bool IsRead { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}

/// <summary>
/// Notification type enumeration
/// </summary>
public enum NotificationType
{
    Application,
    Message,
    Project,
    Deadline,
    Acceptance,
    Rejection,
    System
}
