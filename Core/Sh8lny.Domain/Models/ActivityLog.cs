namespace Sh8lny.Domain.Models;

/// <summary>
/// Activity log entity for audit trail
/// </summary>
public class ActivityLog
{
    // Primary key
    public int LogID { get; set; }

    // Foreign key
    public int? UserID { get; set; }

    // Activity details
    public required string ActivityType { get; set; }
    public string? Description { get; set; }

    // Related entities
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityID { get; set; }

    // Request info
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
}
