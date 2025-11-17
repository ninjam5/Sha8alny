using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.ActivityLogs;
using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(IActivityLogService activityLogService, ILogger<ActivityLogsController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivityLog([FromBody] CreateActivityLogDto dto)
    {
        var result = await _activityLogService.CreateActivityLogAsync(dto);
        return CreatedAtAction(nameof(GetActivityLog), new { id = result.LogID }, result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActivityLog(int id)
    {
        var result = await _activityLogService.GetActivityLogByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActivityLogs([FromBody] ActivityLogFilterDto filter)
    {
        var result = await _activityLogService.GetActivityLogsAsync(filter);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserActivityLogs(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _activityLogService.GetUserActivityLogsAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("recent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRecentActivityLogs([FromQuery] int count = 20)
    {
        var result = await _activityLogService.GetRecentActivityLogsAsync(count);
        return Ok(result);
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActivityStats([FromQuery] int? userId = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _activityLogService.GetActivityStatsAsync(userId, startDate, endDate);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteActivityLog(int id)
    {
        await _activityLogService.DeleteActivityLogAsync(id);
        return NoContent();
    }

    [HttpDelete("cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOldActivityLogs([FromQuery] DateTime beforeDate)
    {
        await _activityLogService.DeleteOldActivityLogsAsync(beforeDate);
        return Ok();
    }
}
