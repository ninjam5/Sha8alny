namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// DTO for creating a new project module (milestone).
/// </summary>
public class CreateProjectModuleDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    /// <summary>
    /// Weight as percentage (0-100) of total project.
    /// </summary>
    public decimal Weight { get; set; }
    
    /// <summary>
    /// Optional estimated duration (e.g., "2 weeks").
    /// </summary>
    public string? EstimatedDuration { get; set; }
}
