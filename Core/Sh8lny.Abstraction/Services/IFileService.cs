using Microsoft.AspNetCore.Http;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for file upload and management operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves an uploaded file to the specified folder.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <param name="folderName">The target folder name (e.g., "profiles", "projects").</param>
    /// <returns>The relative path to the saved file.</returns>
    Task<string> SaveFileAsync(IFormFile file, string folderName);

    /// <summary>
    /// Deletes a file from the server.
    /// </summary>
    /// <param name="filePath">The relative path to the file.</param>
    void DeleteFile(string filePath);

    /// <summary>
    /// Validates if the file type is allowed.
    /// </summary>
    /// <param name="file">The file to validate.</param>
    /// <returns>True if allowed, false otherwise.</returns>
    bool IsValidFileType(IFormFile file);
}
