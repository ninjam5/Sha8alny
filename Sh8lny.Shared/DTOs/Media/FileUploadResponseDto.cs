namespace Sh8lny.Shared.DTOs.Media;

/// <summary>
/// Response DTO for file upload operations.
/// </summary>
public class FileUploadResponseDto
{
    public bool IsSuccess { get; set; }
    public string? FilePath { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? Message { get; set; }

    public static FileUploadResponseDto Success(string filePath, string fileName, long fileSize, string? thumbnailUrl = null)
    {
        return new FileUploadResponseDto
        {
            IsSuccess = true,
            FilePath = filePath,
            ThumbnailUrl = thumbnailUrl,
            FileName = fileName,
            FileSize = fileSize,
            Message = "File uploaded successfully."
        };
    }

    public static FileUploadResponseDto Failure(string message)
    {
        return new FileUploadResponseDto
        {
            IsSuccess = false,
            Message = message
        };
    }
}
