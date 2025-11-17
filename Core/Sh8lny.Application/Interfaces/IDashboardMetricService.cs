using Sh8lny.Application.DTOs.DashboardMetrics;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for dashboard metrics and analytics
/// </summary>
public interface IDashboardMetricService
{
    Task<DashboardMetricDto> CreateDashboardMetricAsync(CreateDashboardMetricDto dto);
    Task<DashboardMetricDto> GetLatestMetricAsync();
    Task<IEnumerable<DashboardMetricDto>> GetMetricsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<PlatformOverviewDto> GetPlatformOverviewAsync();
    Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId);
    Task<CompanyDashboardDto> GetCompanyDashboardAsync(int companyId);
    Task<bool> RefreshMetricsAsync();
}
