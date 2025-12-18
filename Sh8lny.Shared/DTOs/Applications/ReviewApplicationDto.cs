namespace Sh8lny.Shared.DTOs.Applications;

/// <summary>
/// DTO for reviewing (accepting/rejecting) an application.
/// </summary>
public class ReviewApplicationDto
{
    public int ApplicationId { get; set; }
    
    /// <summary>
    /// Status to set: "Accepted" or "Rejected"
    /// </summary>
    public required string Status { get; set; }
    
    /// <summary>
    /// Optional note (e.g., rejection reason)
    /// </summary>
    public string? Note { get; set; }
}
