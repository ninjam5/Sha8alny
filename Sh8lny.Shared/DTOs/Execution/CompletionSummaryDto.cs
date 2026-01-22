namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// DTO containing the completion summary statistics for a finished job.
/// </summary>
public class CompletionSummaryDto
{
    /// <summary>
    /// The application ID.
    /// </summary>
    public int ApplicationId { get; set; }

    /// <summary>
    /// The project ID.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Name of the completed project.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the student who completed the work.
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// The date when work started (application accepted date).
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The date when the job was completed.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Total duration of the job in days.
    /// </summary>
    public int DurationDays { get; set; }

    /// <summary>
    /// Human-readable duration string (e.g., "2 weeks, 3 days").
    /// </summary>
    public string DurationText { get; set; } = string.Empty;

    /// <summary>
    /// Total amount paid (placeholder - to be integrated with payment system).
    /// </summary>
    public decimal TotalPaid { get; set; }

    /// <summary>
    /// Total number of modules completed.
    /// </summary>
    public int TotalModulesCompleted { get; set; }

    /// <summary>
    /// Company's feedback note about the completed work.
    /// </summary>
    public string? CompanyFeedbackNote { get; set; }

    /// <summary>
    /// URL to the final deliverable, if provided.
    /// </summary>
    public string? FinalDeliverableUrl { get; set; }

    /// <summary>
    /// Whether a certificate was generated for this completion.
    /// </summary>
    public bool CertificateGenerated { get; set; }

    /// <summary>
    /// The unique certificate identifier (if generated).
    /// </summary>
    public string? CertificateId { get; set; }
}
