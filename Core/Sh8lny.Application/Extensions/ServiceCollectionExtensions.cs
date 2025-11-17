using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.Interfaces;
using Sh8lny.Application.UseCases.ActivityLogs;
using Sh8lny.Application.UseCases.Applications;
using Sh8lny.Application.UseCases.Auth;
using Sh8lny.Application.UseCases.Certificates;
using Sh8lny.Application.UseCases.Companies;
using Sh8lny.Application.UseCases.DashboardMetrics;
using Sh8lny.Application.UseCases.Messaging;
using Sh8lny.Application.UseCases.Notifications;
using Sh8lny.Application.UseCases.Projects;
using Sh8lny.Application.UseCases.Reviews;
using Sh8lny.Application.UseCases.Students;
using Sh8lny.Application.UseCases.UserSettings;

namespace Sh8lny.Application.Extensions;

/// <summary>
/// Extension methods for configuring application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all application layer services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register core application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IApplicationService, ApplicationService>();

        // Register new module services
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<IDashboardMetricService, DashboardMetricService>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();

        return services;
    }
}
