using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Domain.Interfaces;
using Sh8lny.Persistence.Contexts;
using Sh8lny.Persistence.Repositories;

namespace Sh8lny.Persistence.Extensions;

/// <summary>
/// Extension methods for configuring persistence services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all persistence layer services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration</param>
    /// <param name="environment">Optional environment name for conditional registration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services, 
        IConfiguration configuration,
        string? environment = null)
    {
        // Skip DbContext registration in Testing environment (will be registered by test infrastructure)
        if (environment != "Testing")
        {
            // Register DbContext with SQL Server
            services.AddDbContext<Sha8lnyDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(Sha8lnyDbContext).Assembly.FullName)));
        }

        // Register Unit of Work pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
