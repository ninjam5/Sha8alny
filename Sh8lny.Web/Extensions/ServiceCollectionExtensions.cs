using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.Interfaces;
using Sh8lny.Web.Services;

namespace Sh8lny.Web.Extensions;

/// <summary>
/// Extension methods for configuring infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all infrastructure layer services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register infrastructure services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register HTTP context accessor for CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}
