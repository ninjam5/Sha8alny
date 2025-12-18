using Sh8lny.Domain.Models;
using Sh8lny.Web.DTOs.Base;

namespace Sh8lny.Web.DTOs.Projects;

/// <summary>
/// Response DTO for Project entity.
/// Inherits standard audit properties from BaseDto.
/// </summary>
public class ProjectResponseDto : BaseDto
{
    // Company reference
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }

    // Project details
    public required string ProjectName { get; set; }
    public string? ProjectCode { get; set; }
    public required string Description { get; set; }
    public string? ProjectType { get; set; }

    // Timeline
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime Deadline { get; set; }
    public string? Duration { get; set; }

    // Requirements
    public string? RequiredSkills { get; set; }
    public string? MinAcademicYear { get; set; }
    public int? MaxApplicants { get; set; }

    // Status
    public string Status { get; set; } = string.Empty;
    public bool IsVisible { get; set; }

    // Creator info
    public int CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    // Metrics
    public int ViewCount { get; set; }
    public int ApplicationCount { get; set; }
}
