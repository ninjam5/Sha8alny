using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.ActivityLogs;

#region Activity Log DTOs

/// <summary>
/// DTO for creating an activity log entry
/// </summary>
public class CreateActivityLogDto
{
    public int? UserID { get; set; }

    [Required(ErrorMessage = "Activity type is required")]
    [MaxLength(100, ErrorMessage = "Activity type cannot exceed 100 characters")]
    public string ActivityType { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [MaxLength(50, ErrorMessage = "Related entity type cannot exceed 50 characters")]
    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityID { get; set; }

    [MaxLength(45, ErrorMessage = "IP address cannot exceed 45 characters")]
    public string? IPAddress { get; set; }

    [MaxLength(500, ErrorMessage = "User agent cannot exceed 500 characters")]
    public string? UserAgent { get; set; }
}

/// <summary>
/// DTO for activity log details
/// </summary>
public class ActivityLogDto
{
    public int LogID { get; set; }
    public int? UserID { get; set; }
    public string? UserName { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? RelatedEntityID { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for filtering activity logs
/// </summary>
public class ActivityLogFilterDto
{
    public int? UserID { get; set; }
    public string? ActivityType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

/// <summary>
/// DTO for activity statistics
/// </summary>
public class ActivityStatsDto
{
    public int TotalActivities { get; set; }
    public int TodayActivities { get; set; }
    public int ThisWeekActivities { get; set; }
    public int ThisMonthActivities { get; set; }
    public Dictionary<string, int> ActivityTypeBreakdown { get; set; } = new();
    public List<ActivityTrendDto> DailyTrend { get; set; } = new();
}

/// <summary>
/// DTO for activity trend data
/// </summary>
public class ActivityTrendDto
{
    public DateTime Date { get; set; }
    public int ActivityCount { get; set; }
}

#endregion
