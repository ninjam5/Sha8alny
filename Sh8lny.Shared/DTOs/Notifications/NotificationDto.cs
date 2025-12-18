namespace Sh8lny.Shared.DTOs.Notifications;

/// <summary>
/// Response DTO for notification details.
/// </summary>
public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int? RelatedProjectId { get; set; }
    public int? RelatedApplicationId { get; set; }
    public string? ActionUrl { get; set; }
}
