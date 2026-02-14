using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security;
using Sh8lny.Abstraction.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Sh8lny.Service;

/// <summary>
/// Service for handling file uploads and management.
/// Integrates virus scanning via ClamAV before persisting any file.
/// Images are optimized (resized + WebP conversion) and thumbnails are generated.
/// </summary>
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IVirusScanService _virusScanService;
    private readonly ILogger<FileService> _logger;

    private readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".pdf"
    };
    private readonly HashSet<string> _allowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "application/pdf"
    };
    private readonly HashSet<string> _imageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif"
    };

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const int MaxImageWidth = 1920;
    private const int ThumbnailSize = 300;
    private const int WebPQuality = 80;

    public FileService(
        IWebHostEnvironment environment,
        IVirusScanService virusScanService,
        ILogger<FileService> logger)
    {
        _environment = environment;
        _virusScanService = virusScanService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<FileUploadResult> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("File is empty or null.");

        if (!IsValidFileType(file))
            throw new ArgumentException("Invalid file type. Allowed types: jpg, jpeg, png, gif, pdf.");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"File size exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");

        // ── Virus Scan (before touching disk) ──────────────────
        using var scanStream = new MemoryStream();
        await file.CopyToAsync(scanStream);
        scanStream.Position = 0;

        var isClean = await _virusScanService.IsFileCleanAsync(scanStream, file.FileName);
        if (!isClean)
        {
            _logger.LogWarning("Rejected malicious file upload: '{FileName}'", file.FileName);
            throw new SecurityException($"Malicious file detected. Upload of '{file.FileName}' was rejected.");
        }

        // ── Ensure upload directory exists ─────────────────────
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var baseName = Guid.NewGuid().ToString();

        // ── Route: Image vs. Document ──────────────────────────
        if (_imageExtensions.Contains(extension))
        {
            return await SaveOptimizedImageAsync(scanStream, uploadsFolder, folderName, baseName);
        }
        else
        {
            return await SaveRawFileAsync(scanStream, uploadsFolder, folderName, baseName, extension);
        }
    }

    /// <summary>
    /// Optimizes an image (resize if too wide, convert to WebP) and generates a 300×300 thumbnail.
    /// </summary>
    private async Task<FileUploadResult> SaveOptimizedImageAsync(
        MemoryStream sourceStream, string uploadsFolder, string folderName, string baseName)
    {
        sourceStream.Position = 0;

        using var image = await Image.LoadAsync(sourceStream);

        var originalWidth = image.Width;
        var originalHeight = image.Height;

        // ── Main image: resize if wider than 1920px ────────────
        if (image.Width > MaxImageWidth)
        {
            var ratio = (double)MaxImageWidth / image.Width;
            var newHeight = (int)(image.Height * ratio);
            image.Mutate(x => x.Resize(MaxImageWidth, newHeight));
            _logger.LogInformation("Resized image from {OldW}x{OldH} to {NewW}x{NewH}",
                originalWidth, originalHeight, MaxImageWidth, newHeight);
        }

        var mainFileName = $"{baseName}.webp";
        var mainFilePath = Path.Combine(uploadsFolder, mainFileName);

        var encoder = new WebpEncoder { Quality = WebPQuality };

        await using (var mainStream = new FileStream(mainFilePath, FileMode.Create))
        {
            await image.SaveAsync(mainStream, encoder);
        }

        _logger.LogInformation("Optimized image saved: {Path}", mainFilePath);

        // ── Thumbnail: 300×300 center-crop ─────────────────────
        // Reload from source to avoid quality loss from the already-resized image
        sourceStream.Position = 0;
        using var thumbImage = await Image.LoadAsync(sourceStream);

        thumbImage.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(ThumbnailSize, ThumbnailSize),
                Mode = ResizeMode.Crop,         // Center-crop (cover)
                Position = AnchorPositionMode.Center
            }));

        var thumbFileName = $"{baseName}_thumb.webp";
        var thumbFilePath = Path.Combine(uploadsFolder, thumbFileName);

        await using (var thumbStream = new FileStream(thumbFilePath, FileMode.Create))
        {
            await thumbImage.SaveAsync(thumbStream, encoder);
        }

        _logger.LogInformation("Thumbnail saved: {Path}", thumbFilePath);

        return new FileUploadResult
        {
            FilePath = $"/uploads/{folderName}/{mainFileName}",
            ThumbnailPath = $"/uploads/{folderName}/{thumbFileName}"
        };
    }

    /// <summary>
    /// Saves non-image files (PDFs, etc.) as-is — no compression to avoid corruption.
    /// </summary>
    private async Task<FileUploadResult> SaveRawFileAsync(
        MemoryStream sourceStream, string uploadsFolder, string folderName,
        string baseName, string extension)
    {
        sourceStream.Position = 0;

        var fileName = $"{baseName}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await sourceStream.CopyToAsync(fileStream);
        }

        _logger.LogInformation("Raw file saved: {Path}", filePath);

        return new FileUploadResult
        {
            FilePath = $"/uploads/{folderName}/{fileName}",
            ThumbnailPath = null   // No thumbnail for documents
        };
    }

    /// <inheritdoc />
    public void DeleteFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var relativePath = filePath.TrimStart('/');
        var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
            _logger.LogInformation("Deleted file: {Path}", absolutePath);
        }

        // Also delete associated thumbnail if it exists
        var dir = Path.GetDirectoryName(absolutePath)!;
        var nameWithoutExt = Path.GetFileNameWithoutExtension(absolutePath);
        var thumbPath = Path.Combine(dir, $"{nameWithoutExt}_thumb.webp");

        if (File.Exists(thumbPath))
        {
            File.Delete(thumbPath);
            _logger.LogInformation("Deleted thumbnail: {Path}", thumbPath);
        }
    }

    /// <inheritdoc />
    public bool IsValidFileType(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName);
        var mimeType = file.ContentType;

        return _allowedExtensions.Contains(extension) && _allowedMimeTypes.Contains(mimeType);
    }
}
