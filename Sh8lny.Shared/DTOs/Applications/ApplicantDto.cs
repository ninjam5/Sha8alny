namespace Sh8lny.Shared.DTOs.Applications;

/// <summary>
/// DTO for company to view applicants for a project.
/// </summary>
public class ApplicantDto
{
    public int ApplicationId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentTitle { get; set; }
    public string? Proposal { get; set; }
    public DateTime AppliedDate { get; set; }
}
