using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.DashboardMetrics;
using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardMetricsController : ControllerBase
{
    private readonly IDashboardMetricService _dashboardMetricService;
    private readonly ILogger<DashboardMetricsController> _logger;

    public DashboardMetricsController(IDashboardMetricService dashboardMetricService, ILogger<DashboardMetricsController> logger)
    {
        _dashboardMetricService = dashboardMetricService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDashboardMetric([FromBody] CreateDashboardMetricDto dto)
    {
        var result = await _dashboardMetricService.CreateDashboardMetricAsync(dto);
        return CreatedAtAction(nameof(GetLatestMetric), null, result);
    }

    [HttpGet("latest")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLatestMetric()
    {
        var result = await _dashboardMetricService.GetLatestMetricAsync();
        return Ok(result);
    }

    [HttpGet("date-range")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMetricsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _dashboardMetricService.GetMetricsByDateRangeAsync(startDate, endDate);
        return Ok(result);
    }

    [HttpGet("platform-overview")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPlatformOverview()
    {
        var result = await _dashboardMetricService.GetPlatformOverviewAsync();
        return Ok(result);
    }

    [HttpGet("student-dashboard/{studentId}")]
    public async Task<IActionResult> GetStudentDashboard(int studentId)
    {
        var result = await _dashboardMetricService.GetStudentDashboardAsync(studentId);
        return Ok(result);
    }

    [HttpGet("company-dashboard/{companyId}")]
    public async Task<IActionResult> GetCompanyDashboard(int companyId)
    {
        var result = await _dashboardMetricService.GetCompanyDashboardAsync(companyId);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefreshMetrics()
    {
        await _dashboardMetricService.RefreshMetricsAsync();
        return Ok(new { message = "Metrics refreshed successfully" });
    }
}
