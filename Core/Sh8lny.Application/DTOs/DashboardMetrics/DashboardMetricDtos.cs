using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.DashboardMetrics;

#region Dashboard Metric DTOs

/// <summary>
/// DTO for creating/updating dashboard metrics
/// </summary>
public class CreateDashboardMetricDto
{
    public int TotalStudents { get; set; }
    public int TotalProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int AvailableOpportunities { get; set; }
    public int NewApplicants { get; set; }
    
    [Range(-100, 1000, ErrorMessage = "Activity increase percent must be between -100 and 1000")]
    public decimal ActivityIncreasePercent { get; set; }

    public DateTime? MetricDate { get; set; }
}

/// <summary>
/// DTO for dashboard metric details
/// </summary>
public class DashboardMetricDto
{
    public int MetricID { get; set; }
    public int TotalStudents { get; set; }
    public int TotalProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int AvailableOpportunities { get; set; }
    public int NewApplicants { get; set; }
    public decimal ActivityIncreasePercent { get; set; }
    public DateTime MetricDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for platform overview dashboard
/// </summary>
public class PlatformOverviewDto
{
    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int AcceptedApplications { get; set; }
    public decimal AverageCompanyRating { get; set; }
    public decimal AverageStudentRating { get; set; }
    public int TotalReviews { get; set; }
}

/// <summary>
/// DTO for student dashboard
/// </summary>
public class StudentDashboardDto
{
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int AcceptedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int CompletedProjects { get; set; }
    public int Certificates { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int UnreadMessages { get; set; }
    public int UnreadNotifications { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

/// <summary>
/// DTO for company dashboard
/// </summary>
public class CompanyDashboardDto
{
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalApplications { get; set; }
    public int NewApplications { get; set; }
    public int ProjectsNearDeadline { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int IssuedCertificates { get; set; }
    public int UnreadMessages { get; set; }
    public int UnreadNotifications { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

/// <summary>
/// DTO for recent activity item
/// </summary>
public class RecentActivityDto
{
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? ActionURL { get; set; }
}

#endregion
