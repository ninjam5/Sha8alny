using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Sh8lny.Abstraction.Services;

namespace Sh8lny.Web.Services
{
    /// <summary>
    /// Background worker that performs automated database backups on a configurable schedule.
    /// Also purges expired backups based on the configured retention policy.
    /// </summary>
    public class BackupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackupWorker> _logger;
        private readonly TimeSpan _interval;
        private readonly int _retentionDays;

        public BackupWorker(
            IServiceProvider serviceProvider,
            ILogger<BackupWorker> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Default: every 24 hours, keep backups for 7 days
            var intervalHours = configuration.GetValue("Backup:IntervalHours", 24);
            _retentionDays = configuration.GetValue("Backup:RetentionDays", 7);
            _interval = TimeSpan.FromHours(intervalHours);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "BackupWorker started. Interval: {Interval}h, Retention: {Retention} days.",
                _interval.TotalHours,
                _retentionDays);

            // Wait a short period after startup before the first backup
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            using var timer = new PeriodicTimer(_interval);

            // Run immediately on first tick, then on schedule
            do
            {
                try
                {
                    await RunBackupCycleAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BackupWorker encountered an error during the backup cycle.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunBackupCycleAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

            _logger.LogInformation("Automated backup cycle starting...");

            // 1. Create backup
            var fileName = await backupService.CreateBackupAsync();
            _logger.LogInformation("Scheduled backup completed: {FileName}", fileName);

            // 2. Purge expired backups
            var purged = await backupService.PurgeOldBackupsAsync(_retentionDays);
            _logger.LogInformation("Retention cleanup done. Purged {Count} old backup(s).", purged);
        }
    }
}
