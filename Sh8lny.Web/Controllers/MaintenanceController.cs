using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;

namespace Sh8lny.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(IBackupService backupService, ILogger<MaintenanceController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        /// <summary>
        /// Triggers an on-demand full database backup (Admin only).
        /// </summary>
        [HttpPost("backup")]
        public async Task<IActionResult> CreateBackup()
        {
            _logger.LogInformation("Admin triggered manual backup.");

            try
            {
                var fileName = await _backupService.CreateBackupAsync();
                return Ok(new { Message = "Backup created and verified successfully.", FileName = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Manual backup failed.");
                return StatusCode(500, new { Message = "Backup failed.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Lists all available database backups (Admin only).
        /// </summary>
        [HttpGet("backups")]
        public async Task<IActionResult> ListBackups()
        {
            var backups = await _backupService.ListBackupsAsync();
            return Ok(backups);
        }

        /// <summary>
        /// Purges backups older than the specified number of days (Admin only).
        /// Defaults to 7 days if not provided.
        /// </summary>
        [HttpDelete("backups/purge")]
        public async Task<IActionResult> PurgeBackups([FromQuery] int retentionDays = 7)
        {
            if (retentionDays < 1)
                return BadRequest(new { Message = "Retention days must be at least 1." });

            var deleted = await _backupService.PurgeOldBackupsAsync(retentionDays);
            return Ok(new { Message = $"Purged {deleted} expired backup(s).", DeletedCount = deleted });
        }
    }
}
