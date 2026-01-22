namespace Sh8lny.Shared.DTOs.Reviews;

/// <summary>
/// DTO for returning review details.
/// </summary>
public class ReviewResponseDto
{
    /// <summary>
    /// Review ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the reviewer.
    /// </summary>
    public string ReviewerName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the person/entity being reviewed.
    /// </summary>
    public string RevieweeName { get; set; } = string.Empty;

    /// <summary>
    /// Project name associated with the review.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Rating (1-5).
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Review comment.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// When the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
