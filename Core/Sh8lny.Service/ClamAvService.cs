using Microsoft.Extensions.Logging;
using Sh8lny.Abstraction.Services;

namespace Sh8lny.Service;

/// <summary>
/// Virus scanning stub — ClamAV has been disabled.
/// Always reports files as clean to keep the IVirusScanService contract valid.
/// </summary>
public class ClamAvService : IVirusScanService
{
    private readonly ILogger<ClamAvService> _logger;

    public ClamAvService(ILogger<ClamAvService> logger)
    {
        _logger = logger;
        _logger.LogWarning("Virus scanning is DISABLED. ClamAV integration has been removed.");
    }

    /// <inheritdoc />
    public Task<bool> IsFileCleanAsync(Stream fileStream, string fileName = "unknown")
    {
        _logger.LogWarning("Virus scanning is disabled. Skipping check for file: {FileName}", fileName);
        return Task.FromResult(true);
    }
}
