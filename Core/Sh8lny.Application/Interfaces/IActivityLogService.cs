using Sh8lny.Application.DTOs.ActivityLogs;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for activity logging and analytics
/// </summary>
public interface IActivityLogService
{
    Task<ActivityLogDto> CreateActivityLogAsync(CreateActivityLogDto dto);
    Task<ActivityLogDto> GetActivityLogByIdAsync(int logId);
    Task<IEnumerable<ActivityLogDto>> GetActivityLogsAsync(ActivityLogFilterDto filter);
    Task<IEnumerable<ActivityLogDto>> GetUserActivityLogsAsync(int userId, int page = 1, int pageSize = 50);
    Task<IEnumerable<ActivityLogDto>> GetRecentActivityLogsAsync(int count = 20);
    Task<ActivityStatsDto> GetActivityStatsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> DeleteActivityLogAsync(int logId);
    Task<bool> DeleteOldActivityLogsAsync(DateTime beforeDate);
}
