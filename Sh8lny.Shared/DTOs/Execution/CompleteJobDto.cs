namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// DTO for completing a job/project by the company.
/// </summary>
public class CompleteJobDto
{
    /// <summary>
    /// The application ID to complete.
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// Optional feedback note from the company about the completed work.
    /// </summary>
    public string? CompanyFeedbackNote { get; set; }

    /// <summary>
    /// Optional URL to the final deliverable (e.g., GitHub repo, deployed site, etc.).
    /// </summary>
    public string? FinalDeliverableUrl { get; set; }
}
