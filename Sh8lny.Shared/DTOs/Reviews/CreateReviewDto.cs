using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Shared.DTOs.Reviews;

/// <summary>
/// DTO for creating a review (for both Student and Company reviews).
/// </summary>
public class CreateReviewDto
{
    /// <summary>
    /// The application ID to review (completed job).
    /// </summary>
    [Required]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Rating from 1 to 5.
    /// </summary>
    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment/feedback about the work.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters.")]
    public string? Comment { get; set; }
}
