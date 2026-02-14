namespace Sh8lny.Abstraction.Services
{
    public interface IBackupService
    {
        /// <summary>
        /// Creates a full database backup and verifies its integrity.
        /// Returns the backup file path on success.
        /// </summary>
        Task<string> CreateBackupAsync();

        /// <summary>
        /// Lists all available backup files with metadata.
        /// </summary>
        Task<IEnumerable<BackupFileInfo>> ListBackupsAsync();

        /// <summary>
        /// Deletes backups older than the specified retention period.
        /// Returns the number of deleted files.
        /// </summary>
        Task<int> PurgeOldBackupsAsync(int retentionDays);
    }

    public class BackupFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
