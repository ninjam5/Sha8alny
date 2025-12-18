using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sh8lny.Abstraction.Services;

namespace Sh8lny.Service;

/// <summary>
/// Service for handling file uploads and management.
/// </summary>
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".pdf"
    };
    private readonly HashSet<string> _allowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "application/pdf"
    };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <inheritdoc />
    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null.");
        }

        if (!IsValidFileType(file))
        {
            throw new ArgumentException("Invalid file type. Allowed types: jpg, jpeg, png, gif, pdf.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
        }

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        // Create folder path
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
        
        // Ensure directory exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Full file path
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        // Return relative path for database storage
        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    /// <inheritdoc />
    public void DeleteFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        // Convert relative path to absolute path
        var relativePath = filePath.TrimStart('/');
        var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }

    /// <inheritdoc />
    public bool IsValidFileType(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        var mimeType = file.ContentType;

        return _allowedExtensions.Contains(extension) && _allowedMimeTypes.Contains(mimeType);
    }
}
