using Sh8lny.Application.DTOs.ActivityLogs;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.ActivityLogs;

/// <summary>
/// Service for activity logging and analytics
/// </summary>
public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ActivityLogDto> CreateActivityLogAsync(CreateActivityLogDto dto)
    {
        // Create new activity log entity
        var activityLog = new ActivityLog
        {
            UserID = dto.UserID,
            ActivityType = dto.ActivityType,
            Description = dto.Description,
            RelatedEntityType = dto.RelatedEntityType,
            RelatedEntityID = dto.RelatedEntityID,
            IPAddress = dto.IPAddress,
            UserAgent = dto.UserAgent,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync();

        return MapToActivityLogDto(activityLog);
    }

    public async Task<ActivityLogDto> GetActivityLogByIdAsync(int logId)
    {
        var log = await _unitOfWork.ActivityLogs.GetByIdAsync(logId);
        if (log == null)
            throw new NotFoundException("ActivityLog", logId);

        // Load user details if exists
        if (log.UserID.HasValue)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(log.UserID.Value);
            if (user != null)
                log.User = user;
        }

        return MapToActivityLogDto(log);
    }

    public async Task<IEnumerable<ActivityLogDto>> GetActivityLogsAsync(ActivityLogFilterDto filter)
    {
        // Get all activity logs and apply filters
        var query = (await _unitOfWork.ActivityLogs.GetAllAsync()).AsQueryable();

        // Apply filters
        if (filter.UserID.HasValue)
            query = query.Where(al => al.UserID == filter.UserID.Value);

        if (!string.IsNullOrEmpty(filter.ActivityType))
            query = query.Where(al => al.ActivityType == filter.ActivityType);

        if (filter.StartDate.HasValue)
            query = query.Where(al => al.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(al => al.CreatedAt <= filter.EndDate.Value);

        // Order by most recent
        query = query.OrderByDescending(al => al.CreatedAt);

        // Apply pagination
        var logs = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        // Load user details
        var userIds = logs.Where(l => l.UserID.HasValue).Select(l => l.UserID!.Value).Distinct().ToList();
        if (userIds.Any())
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDict = users.Where(u => userIds.Contains(u.UserID)).ToDictionary(u => u.UserID, u => u);
            
            foreach (var log in logs)
            {
                if (log.UserID.HasValue && userDict.ContainsKey(log.UserID.Value))
                    log.User = userDict[log.UserID.Value];
            }
        }

        return logs.Select(MapToActivityLogDto);
    }

    public async Task<IEnumerable<ActivityLogDto>> GetUserActivityLogsAsync(int userId, int page = 1, int pageSize = 50)
    {
        // Get logs for specific user
        var logs = await _unitOfWork.ActivityLogs.GetByUserIdAsync(userId);

        // Apply pagination
        var paginatedLogs = logs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Load user details
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user != null)
        {
            foreach (var log in paginatedLogs)
            {
                log.User = user;
            }
        }

        return paginatedLogs.Select(MapToActivityLogDto);
    }

    public async Task<IEnumerable<ActivityLogDto>> GetRecentActivityLogsAsync(int count = 20)
    {
        // Get most recent activities across platform
        var logs = await _unitOfWork.ActivityLogs.GetRecentActivitiesAsync(count);

        return logs.Select(MapToActivityLogDto);
    }

    public async Task<ActivityStatsDto> GetActivityStatsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        // Get all activity logs
        var allLogs = (await _unitOfWork.ActivityLogs.GetAllAsync()).AsQueryable();

        // Filter by user if specified
        if (userId.HasValue)
            allLogs = allLogs.Where(al => al.UserID == userId.Value);

        // Filter by date range if specified
        if (startDate.HasValue)
            allLogs = allLogs.Where(al => al.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            allLogs = allLogs.Where(al => al.CreatedAt <= endDate.Value);

        var logsList = allLogs.ToList();

        // Calculate statistics
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var stats = new ActivityStatsDto
        {
            TotalActivities = logsList.Count,
            TodayActivities = logsList.Count(al => al.CreatedAt >= todayStart),
            ThisWeekActivities = logsList.Count(al => al.CreatedAt >= weekStart),
            ThisMonthActivities = logsList.Count(al => al.CreatedAt >= monthStart)
        };

        // Activity type breakdown
        stats.ActivityTypeBreakdown = logsList
            .GroupBy(al => al.ActivityType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Daily trend (last 30 days)
        var thirtyDaysAgo = now.Date.AddDays(-30);
        stats.DailyTrend = logsList
            .Where(al => al.CreatedAt >= thirtyDaysAgo)
            .GroupBy(al => al.CreatedAt.Date)
            .Select(g => new ActivityTrendDto
            {
                Date = g.Key,
                ActivityCount = g.Count()
            })
            .OrderBy(t => t.Date)
            .ToList();

        return stats;
    }

    public async Task<bool> DeleteActivityLogAsync(int logId)
    {
        // Admin function to delete specific log
        var log = await _unitOfWork.ActivityLogs.GetByIdAsync(logId);
        if (log == null)
            throw new NotFoundException("ActivityLog", logId);

        await _unitOfWork.ActivityLogs.DeleteAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteOldActivityLogsAsync(DateTime beforeDate)
    {
        // Cleanup old logs for data retention compliance
        var allLogs = await _unitOfWork.ActivityLogs.GetAllAsync();
        var oldLogs = allLogs.Where(al => al.CreatedAt < beforeDate).ToList();

        if (oldLogs.Any())
        {
            foreach (var log in oldLogs)
            {
                await _unitOfWork.ActivityLogs.DeleteAsync(log);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        return true;
    }

    private ActivityLogDto MapToActivityLogDto(ActivityLog log)
    {
        return new ActivityLogDto
        {
            LogID = log.LogID,
            UserID = log.UserID,
            UserName = log.User?.Email,
            ActivityType = log.ActivityType,
            Description = log.Description,
            RelatedEntityType = log.RelatedEntityType,
            RelatedEntityID = log.RelatedEntityID,
            IPAddress = log.IPAddress,
            UserAgent = log.UserAgent,
            CreatedAt = log.CreatedAt
        };
    }
}
