namespace Sh8lny.Shared.DTOs.Projects;

/// <summary>
/// DTO for skill information in project responses.
/// </summary>
public class ProjectSkillDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsRequired { get; set; }
}

/// <summary>
/// Response DTO for Project entity with full details.
/// </summary>
public class ProjectResponseDto
{
    public int Id { get; set; }
    
    // Company reference
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyLogo { get; set; }

    // Project details
    public required string Title { get; set; }
    public string? ProjectCode { get; set; }
    public required string Description { get; set; }
    public string? ProjectType { get; set; }

    // Timeline
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime Deadline { get; set; }
    public string? Duration { get; set; }

    // Requirements
    public List<ProjectSkillDto> RequiredSkills { get; set; } = new();
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

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
