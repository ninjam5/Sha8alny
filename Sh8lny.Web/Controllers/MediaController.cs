using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Media;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for media/file upload operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IFileService _fileService;

    public MediaController(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// Uploads a profile picture.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The URL path to the uploaded file.</returns>
    [HttpPost("upload/profile")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadProfilePicture(IFormFile file)
    {
        return await UploadFile(file, "profiles");
    }

    /// <summary>
    /// Uploads a company logo.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The URL path to the uploaded file.</returns>
    [HttpPost("upload/logo")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadCompanyLogo(IFormFile file)
    {
        return await UploadFile(file, "logos");
    }

    /// <summary>
    /// Uploads a project attachment.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <returns>The URL path to the uploaded file.</returns>
    [HttpPost("upload/project")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadProjectAttachment(IFormFile file)
    {
        return await UploadFile(file, "projects");
    }

    /// <summary>
    /// Uploads a certificate image.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The URL path to the uploaded file.</returns>
    [HttpPost("upload/certificate")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadCertificate(IFormFile file)
    {
        return await UploadFile(file, "certificates");
    }

    /// <summary>
    /// Generic upload endpoint with folder specification.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="folder">The target folder name.</param>
    /// <returns>The URL path to the uploaded file.</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResponseDto>> Upload(IFormFile file, [FromQuery] string folder = "general")
    {
        // Sanitize folder name to prevent directory traversal
        var sanitizedFolder = SanitizeFolderName(folder);
        return await UploadFile(file, sanitizedFolder);
    }

    /// <summary>
    /// Deletes a previously uploaded file.
    /// </summary>
    /// <param name="filePath">The relative path to the file.</param>
    /// <returns>Success or failure response.</returns>
    [HttpDelete]
    public ActionResult<FileUploadResponseDto> DeleteFile([FromQuery] string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest(FileUploadResponseDto.Failure("File path is required."));
            }

            // Validate that the path is within uploads folder
            if (!filePath.StartsWith("/uploads/"))
            {
                return BadRequest(FileUploadResponseDto.Failure("Invalid file path."));
            }

            _fileService.DeleteFile(filePath);

            return Ok(new FileUploadResponseDto
            {
                IsSuccess = true,
                Message = "File deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(FileUploadResponseDto.Failure($"Failed to delete file: {ex.Message}"));
        }
    }

    /// <summary>
    /// Helper method to handle file upload logic.
    /// </summary>
    private async Task<ActionResult<FileUploadResponseDto>> UploadFile(IFormFile file, string folderName)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(FileUploadResponseDto.Failure("No file provided or file is empty."));
            }

            if (!_fileService.IsValidFileType(file))
            {
                return BadRequest(FileUploadResponseDto.Failure("Invalid file type. Allowed types: jpg, jpeg, png, gif, pdf."));
            }

            var result = await _fileService.SaveFileAsync(file, folderName);

            return Ok(FileUploadResponseDto.Success(result.FilePath, file.FileName, file.Length, result.ThumbnailPath));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(FileUploadResponseDto.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, FileUploadResponseDto.Failure($"An error occurred while uploading the file: {ex.Message}"));
        }
    }

    /// <summary>
    /// Sanitizes folder name to prevent directory traversal attacks.
    /// </summary>
    private static string SanitizeFolderName(string folder)
    {
        // Remove any path separators and dangerous characters
        var sanitized = folder
            .Replace("/", "")
            .Replace("\\", "")
            .Replace("..", "")
            .Trim();

        // Default to "general" if empty after sanitization
        return string.IsNullOrWhiteSpace(sanitized) ? "general" : sanitized.ToLowerInvariant();
    }
}
