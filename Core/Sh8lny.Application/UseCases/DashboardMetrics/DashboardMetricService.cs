using Sh8lny.Application.DTOs.DashboardMetrics;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.DashboardMetrics;

/// <summary>
/// Service for dashboard metrics and analytics
/// </summary>
public class DashboardMetricService : IDashboardMetricService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int CacheExpirationMinutes = 30; // Cache metrics for 30 minutes

    public DashboardMetricService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardMetricDto> CreateDashboardMetricAsync(CreateDashboardMetricDto dto)
    {
        var metric = new DashboardMetric
        {
            TotalStudents = dto.TotalStudents,
            TotalProjects = dto.TotalProjects,
            CompletedProjects = dto.CompletedProjects,
            AvailableOpportunities = dto.AvailableOpportunities,
            NewApplicants = dto.NewApplicants,
            ActivityIncreasePercent = dto.ActivityIncreasePercent,
            MetricDate = dto.MetricDate ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DashboardMetrics.AddAsync(metric);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(metric);
    }

    public async Task<DashboardMetricDto> GetLatestMetricAsync()
    {
        var metrics = await _unitOfWork.DashboardMetrics.GetAllAsync();
        var latestMetric = metrics.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

        if (latestMetric == null)
        {
            // No cached metrics, create one
            await RefreshMetricsAsync();
            metrics = await _unitOfWork.DashboardMetrics.GetAllAsync();
            latestMetric = metrics.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
        }
        else if ((DateTime.UtcNow - latestMetric.CreatedAt).TotalMinutes > CacheExpirationMinutes)
        {
            // Cache expired, refresh
            await RefreshMetricsAsync();
            metrics = await _unitOfWork.DashboardMetrics.GetAllAsync();
            latestMetric = metrics.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
        }

        if (latestMetric == null)
            throw new NotFoundException(nameof(DashboardMetric), "latest");

        return MapToDto(latestMetric);
    }

    public async Task<IEnumerable<DashboardMetricDto>> GetMetricsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var metrics = await _unitOfWork.DashboardMetrics.FindAsync(
            m => m.MetricDate >= startDate && m.MetricDate <= endDate
        );

        return metrics
            .OrderBy(m => m.MetricDate)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PlatformOverviewDto> GetPlatformOverviewAsync()
    {
        // Get all required data in parallel
        var totalStudents = await _unitOfWork.Students.CountAsync();
        var activeStudents = await _unitOfWork.Students.CountAsync(s => s.Status == StudentStatus.Active);
        var totalCompanies = await _unitOfWork.Companies.CountAsync();
        var activeCompanies = totalCompanies; // Company entity doesn't have Status property, using total count

        var totalProjects = await _unitOfWork.Projects.CountAsync();
        var activeProjects = await _unitOfWork.Projects.CountAsync(p => p.Status == ProjectStatus.Active);
        var completedProjects = await _unitOfWork.Projects.CountAsync(p => p.Status == ProjectStatus.Complete);

        var totalApplications = await _unitOfWork.Applications.CountAsync();
        var pendingApplications = await _unitOfWork.Applications.CountAsync(a => a.Status == ApplicationStatus.Pending);
        var acceptedApplications = await _unitOfWork.Applications.CountAsync(a => a.Status == ApplicationStatus.Accepted);

        // Calculate average ratings
        var approvedCompanyReviews = await _unitOfWork.CompanyReviews.FindAsync(r => r.Status == ReviewStatus.Approved);
        var companyReviewList = approvedCompanyReviews.ToList();
        var averageCompanyRating = companyReviewList.Any() ? Math.Round(companyReviewList.Average(r => r.Rating), 2) : 0;

        var approvedStudentReviews = await _unitOfWork.StudentReviews.FindAsync(r => r.Status == ReviewStatus.Approved);
        var studentReviewList = approvedStudentReviews.ToList();
        var averageStudentRating = studentReviewList.Any() ? Math.Round(studentReviewList.Average(r => r.Rating), 2) : 0;

        var totalReviews = companyReviewList.Count + studentReviewList.Count;

        return new PlatformOverviewDto
        {
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            TotalCompanies = totalCompanies,
            ActiveCompanies = activeCompanies,
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            CompletedProjects = completedProjects,
            TotalApplications = totalApplications,
            PendingApplications = pendingApplications,
            AcceptedApplications = acceptedApplications,
            AverageCompanyRating = averageCompanyRating,
            AverageStudentRating = averageStudentRating,
            TotalReviews = totalReviews
        };
    }

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(studentId);
        if (student == null)
            throw new NotFoundException(nameof(Student), studentId);

        // Get student applications
        var applications = await _unitOfWork.Applications.FindAsync(a => a.StudentID == studentId);
        var applicationList = applications.ToList();

        var totalApplications = applicationList.Count;
        var pendingApplications = applicationList.Count(a => a.Status == ApplicationStatus.Pending);
        var acceptedApplications = applicationList.Count(a => a.Status == ApplicationStatus.Accepted);
        var rejectedApplications = applicationList.Count(a => a.Status == ApplicationStatus.Rejected);

        // Get completed projects
        var completedOpportunities = await _unitOfWork.CompletedOpportunities.FindAsync(co => co.StudentID == studentId);
        var completedProjects = completedOpportunities.Count();

        // Get certificates
        var certificates = await _unitOfWork.Certificates.FindAsync(c => c.StudentID == studentId);
        var certificateCount = certificates.Count();

        // Get reviews
        var reviews = await _unitOfWork.StudentReviews.FindAsync(r => r.StudentID == studentId && r.Status == ReviewStatus.Approved);
        var reviewList = reviews.ToList();
        var totalReviews = reviewList.Count;
        var averageRating = totalReviews > 0 ? Math.Round(reviewList.Average(r => r.Rating), 2) : 0;

        // Get unread messages and notifications
        var conversations = await _unitOfWork.ConversationParticipants.FindAsync(cp => cp.UserID == student.UserID);
        var conversationIds = conversations.Select(cp => cp.ConversationID).ToList();
        var unreadMessages = 0;
        foreach (var convId in conversationIds)
        {
            var messages = await _unitOfWork.Messages.FindAsync(m => m.ConversationID == convId && !m.IsRead && m.SenderID != student.UserID);
            unreadMessages += messages.Count();
        }

        var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserID == student.UserID && !n.IsRead);
        var unreadNotifications = notifications.Count();

        // Get recent activities (from activity logs)
        var activityLogs = await _unitOfWork.ActivityLogs.FindAsync(a => a.UserID == student.UserID);
        var recentActivities = activityLogs
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new RecentActivityDto
            {
                ActivityType = a.ActivityType,
                Description = a.Description ?? string.Empty,
                CreatedAt = a.CreatedAt,
                ActionURL = null // Can be enhanced to include relevant URLs
            })
            .ToList();

        return new StudentDashboardDto
        {
            StudentID = studentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            TotalApplications = totalApplications,
            PendingApplications = pendingApplications,
            AcceptedApplications = acceptedApplications,
            RejectedApplications = rejectedApplications,
            CompletedProjects = completedProjects,
            Certificates = certificateCount,
            AverageRating = averageRating,
            TotalReviews = totalReviews,
            UnreadMessages = unreadMessages,
            UnreadNotifications = unreadNotifications,
            RecentActivities = recentActivities
        };
    }

    public async Task<CompanyDashboardDto> GetCompanyDashboardAsync(int companyId)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
            throw new NotFoundException(nameof(Company), companyId);

        // Get company projects
        var projects = await _unitOfWork.Projects.FindAsync(p => p.CompanyID == companyId);
        var projectList = projects.ToList();

        var totalProjects = projectList.Count;
        var activeProjects = projectList.Count(p => p.Status == ProjectStatus.Active);
        var completedProjects = projectList.Count(p => p.Status == ProjectStatus.Complete);

        // Get applications for company projects
        var projectIds = projectList.Select(p => p.ProjectID).ToList();
        var allApplications = new List<Domain.Entities.Application>();
        foreach (var projectId in projectIds)
        {
            var apps = await _unitOfWork.Applications.FindAsync(a => a.ProjectID == projectId);
            allApplications.AddRange(apps);
        }

        var totalApplications = allApplications.Count;
        var newApplications = allApplications.Count(a => a.Status == ApplicationStatus.Pending);

        // Get projects near deadline (within 7 days)
        var sevenDaysFromNow = DateTime.UtcNow.AddDays(7);
        var projectsNearDeadline = projectList.Count(p => 
            p.Status == ProjectStatus.Active && 
            p.EndDate.HasValue && 
            p.EndDate.Value <= sevenDaysFromNow && 
            p.EndDate.Value >= DateTime.UtcNow);

        // Get reviews
        var reviews = await _unitOfWork.CompanyReviews.FindAsync(r => r.CompanyID == companyId && r.Status == ReviewStatus.Approved);
        var reviewList = reviews.ToList();
        var totalReviews = reviewList.Count;
        var averageRating = totalReviews > 0 ? Math.Round(reviewList.Average(r => r.Rating), 2) : 0;

        // Get issued certificates
        var certificates = await _unitOfWork.Certificates.FindAsync(c => c.CompanyID == companyId);
        var issuedCertificates = certificates.Count();

        // Get unread messages and notifications
        var conversations = await _unitOfWork.ConversationParticipants.FindAsync(cp => cp.UserID == company.UserID);
        var conversationIds = conversations.Select(cp => cp.ConversationID).ToList();
        var unreadMessages = 0;
        foreach (var convId in conversationIds)
        {
            var messages = await _unitOfWork.Messages.FindAsync(m => m.ConversationID == convId && !m.IsRead && m.SenderID != company.UserID);
            unreadMessages += messages.Count();
        }

        var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserID == company.UserID && !n.IsRead);
        var unreadNotifications = notifications.Count();

        // Get recent activities (from activity logs)
        var activityLogs = await _unitOfWork.ActivityLogs.FindAsync(a => a.UserID == company.UserID);
        var recentActivities = activityLogs
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new RecentActivityDto
            {
                ActivityType = a.ActivityType,
                Description = a.Description ?? string.Empty,
                CreatedAt = a.CreatedAt,
                ActionURL = null // Can be enhanced to include relevant URLs
            })
            .ToList();

        return new CompanyDashboardDto
        {
            CompanyID = companyId,
            CompanyName = company.CompanyName,
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            CompletedProjects = completedProjects,
            TotalApplications = totalApplications,
            NewApplications = newApplications,
            ProjectsNearDeadline = projectsNearDeadline,
            AverageRating = averageRating,
            TotalReviews = totalReviews,
            IssuedCertificates = issuedCertificates,
            UnreadMessages = unreadMessages,
            UnreadNotifications = unreadNotifications,
            RecentActivities = recentActivities
        };
    }

    public async Task<bool> RefreshMetricsAsync()
    {
        // Calculate current platform metrics
        var totalStudents = await _unitOfWork.Students.CountAsync();
        var totalProjects = await _unitOfWork.Projects.CountAsync();
        var completedProjects = await _unitOfWork.Projects.CountAsync(p => p.Status == ProjectStatus.Complete);
        var availableOpportunities = await _unitOfWork.Projects.CountAsync(p => p.Status == ProjectStatus.Active);
        
        // Get new applicants (applications in last 30 days)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var recentApplications = await _unitOfWork.Applications.FindAsync(a => a.AppliedAt >= thirtyDaysAgo);
        var newApplicants = recentApplications.Select(a => a.StudentID).Distinct().Count();

        // Calculate activity increase percent by comparing to previous metric
        var previousMetrics = await _unitOfWork.DashboardMetrics.GetAllAsync();
        var previousMetric = previousMetrics.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
        
        decimal activityIncreasePercent = 0;
        if (previousMetric != null)
        {
            var previousActivity = previousMetric.NewApplicants + previousMetric.TotalProjects;
            var currentActivity = newApplicants + totalProjects;
            
            if (previousActivity > 0)
            {
                activityIncreasePercent = Math.Round(((decimal)(currentActivity - previousActivity) / previousActivity) * 100, 2);
            }
        }

        // Create new metric snapshot
        var dto = new CreateDashboardMetricDto
        {
            TotalStudents = totalStudents,
            TotalProjects = totalProjects,
            CompletedProjects = completedProjects,
            AvailableOpportunities = availableOpportunities,
            NewApplicants = newApplicants,
            ActivityIncreasePercent = activityIncreasePercent,
            MetricDate = DateTime.UtcNow
        };

        await CreateDashboardMetricAsync(dto);
        return true;
    }

    #region Helper Methods

    private static DashboardMetricDto MapToDto(DashboardMetric metric)
    {
        return new DashboardMetricDto
        {
            MetricID = metric.MetricID,
            TotalStudents = metric.TotalStudents,
            TotalProjects = metric.TotalProjects,
            CompletedProjects = metric.CompletedProjects,
            AvailableOpportunities = metric.AvailableOpportunities,
            NewApplicants = metric.NewApplicants,
            ActivityIncreasePercent = metric.ActivityIncreasePercent,
            MetricDate = metric.MetricDate,
            CreatedAt = metric.CreatedAt
        };
    }

    #endregion
}
