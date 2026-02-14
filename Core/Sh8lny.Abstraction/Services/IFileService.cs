using Microsoft.AspNetCore.Http;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Result returned by file upload operations.
/// </summary>
public class FileUploadResult
{
    /// <summary>Relative URL to the main (optimized) file.</summary>
    public required string FilePath { get; set; }

    /// <summary>Relative URL to the thumbnail (null for non-image files).</summary>
    public string? ThumbnailPath { get; set; }
}

/// <summary>
/// Interface for file upload and management operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Saves an uploaded file to the specified folder.
    /// Images are optimized (resized, converted to WebP) and a thumbnail is generated.
    /// Non-image files are saved as-is.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <param name="folderName">The target folder name (e.g., "profiles", "projects").</param>
    /// <returns>Upload result containing main file path and optional thumbnail path.</returns>
    Task<FileUploadResult> SaveFileAsync(IFormFile file, string folderName);

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
