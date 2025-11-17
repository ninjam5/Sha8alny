using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sh8lny.Persistence.Contexts;
using Sh8lny.Web;
using System.Data.Common;

namespace Sh8lny.IntegrationTests.Fixtures;

/// <summary>
/// Custom WebApplicationFactory for integration testing with SQLite file-based database
/// Uses a singleton DbConnection object shared across all scopes for data persistence
/// </summary>
public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the original DbContext registration
            services.RemoveAll(typeof(DbContextOptions<Sha8lnyDbContext>));
            services.RemoveAll(typeof(Sha8lnyDbContext));

            // Register the DbContext to use the *singleton connection object*
            // Use a lambda that captures 'this' to access the connection
            services.AddDbContext<Sha8lnyDbContext>((sp, options) =>
            {
                // By the time a DbContext is requested, InitializeAsync will have run
                // and _connection will be initialized
                if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
                {
                    throw new InvalidOperationException("Database connection not initialized. IAsyncLifetime.InitializeAsync should run first.");
                }
                
                options.UseSqlite(_connection);
                options.EnableSensitiveDataLogging();
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // Create, open, and share the connection
        // Use Cache=Shared to ensure proper sharing across threads
        var connectionString = $"DataSource={_dbPath};Cache=Shared";
        _connection = new SqliteConnection(connectionString);
        await _connection.OpenAsync();

        // Enable WAL mode for better concurrent access
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "PRAGMA journal_mode=WAL;";
            await command.ExecuteNonQueryAsync();
        }

        // Create the DbContext and apply the schema ONCE
        var options = new DbContextOptionsBuilder<Sha8lnyDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var dbContext = new Sha8lnyDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
    }

    // This is the IAsyncLifetime dispose method
    public new async Task DisposeAsync()
    {
        // Close and dispose the connection
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        // Clear the pool to release file locks before deleting
        SqliteConnection.ClearAllPools();
        
        // Delete the temporary database file
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }

        await base.DisposeAsync();
    }
}
