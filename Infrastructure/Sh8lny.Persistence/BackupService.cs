using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sh8lny.Abstraction.Services;
using Sh8lny.Persistence.Contexts;

namespace Sh8lny.Persistence
{
    public class BackupService : IBackupService
    {
        private readonly Sha8lnyDbContext _dbContext;
        private readonly ILogger<BackupService> _logger;

        // Inside the container this maps to the host's ./backups folder
        private const string BackupDirectory = "/var/opt/mssql/backups";
        private const string DatabaseName = "Sh8lnyDB";

        public BackupService(Sha8lnyDbContext dbContext, ILogger<BackupService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<string> CreateBackupAsync()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{DatabaseName}_backup_{timestamp}.bak";
            var filePath = $"{BackupDirectory}/{fileName}";

            _logger.LogInformation("Starting database backup to {FilePath}...", filePath);

            // Ensure the backup directory exists inside the container
            var ensureDirSql = $"EXEC xp_create_subdir N'{BackupDirectory}'";
            await _dbContext.Database.ExecuteSqlRawAsync(ensureDirSql);

            // Full database backup with compression and checksum
            var backupSql = $@"
                BACKUP DATABASE [{DatabaseName}]
                TO DISK = N'{filePath}'
                WITH FORMAT,
                     INIT,
                     NAME = N'{DatabaseName} Full Backup {timestamp}',
                     COMPRESSION,
                     CHECKSUM,
                     STATS = 10";

            await _dbContext.Database.ExecuteSqlRawAsync(backupSql);
            _logger.LogInformation("Backup file created: {FilePath}", filePath);

            // Verify backup integrity
            var verifySql = $@"
                RESTORE VERIFYONLY
                FROM DISK = N'{filePath}'
                WITH CHECKSUM";

            await _dbContext.Database.ExecuteSqlRawAsync(verifySql);
            _logger.LogInformation("Backup integrity verified successfully: {FileName}", fileName);

            return fileName;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BackupFileInfo>> ListBackupsAsync()
        {
            // Query SQL Server's msdb for backup history of this database
            var backups = new List<BackupFileInfo>();

            using var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    bmf.physical_device_name AS FilePath,
                    bs.backup_size          AS SizeInBytes,
                    bs.backup_finish_date   AS FinishedAt
                FROM msdb.dbo.backupset bs
                INNER JOIN msdb.dbo.backupmediafamily bmf
                    ON bs.media_set_id = bmf.media_set_id
                WHERE bs.database_name = @dbName
                  AND bs.type = 'D'
                ORDER BY bs.backup_finish_date DESC";

            var param = command.CreateParameter();
            param.ParameterName = "@dbName";
            param.Value = DatabaseName;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fullPath = reader.GetString(0);
                backups.Add(new BackupFileInfo
                {
                    FileName = Path.GetFileName(fullPath),
                    SizeInBytes = reader.IsDBNull(1) ? 0 : Convert.ToInt64(reader.GetDecimal(1)),
                    CreatedAtUtc = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2)
                });
            }

            return backups;
        }

        /// <inheritdoc />
        public async Task<int> PurgeOldBackupsAsync(int retentionDays)
        {
            var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
            var deleted = 0;

            var backups = await ListBackupsAsync();
            var expiredBackups = backups.Where(b => b.CreatedAtUtc < cutoff).ToList();

            foreach (var backup in expiredBackups)
            {
                var fullPath = $"{BackupDirectory}/{backup.FileName}";
                try
                {
                    // Use xp_delete_file to remove the backup from within SQL Server
                    // Parameters: file type (0 = backup), folder, extension, cutoff date, subfolder flag
                    var deleteSql = $"EXEC xp_delete_files N'{fullPath}'";

                    // Fallback: delete via the file system if running in the same container filesystem
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        deleted++;
                        _logger.LogInformation("Purged expired backup: {FileName}", backup.FileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete expired backup: {FileName}", backup.FileName);
                }
            }

            _logger.LogInformation("Purged {Count} expired backup(s) older than {Days} days.", deleted, retentionDays);
            return deleted;
        }
    }
}
