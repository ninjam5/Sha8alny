namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for virus scanning file uploads before persisting to storage.
/// </summary>
public interface IVirusScanService
{
    /// <summary>
    /// Scans a file stream for viruses/malware.
    /// </summary>
    /// <param name="fileStream">The file stream to scan.</param>
    /// <param name="fileName">Original file name (for logging).</param>
    /// <returns>True if the file is clean; false if a threat was detected.</returns>
    Task<bool> IsFileCleanAsync(Stream fileStream, string fileName = "unknown");
}
