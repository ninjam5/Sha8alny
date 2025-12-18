namespace Sh8lny.Shared.DTOs.Projects;

/// <summary>
/// DTO for creating a new project/opportunity.
/// </summary>
public class CreateProjectDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? ProjectType { get; set; }
    
    // Timeline
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime Deadline { get; set; }
    public string? Duration { get; set; }
    
    // Requirements
    public List<int> RequiredSkillIds { get; set; } = new();
    public string? MinAcademicYear { get; set; }
    public int? MaxApplicants { get; set; }
    
    // Visibility
    public bool IsVisible { get; set; } = true;
}
