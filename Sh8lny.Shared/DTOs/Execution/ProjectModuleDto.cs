namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// Response DTO for project module details.
/// </summary>
public class ProjectModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Weight { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? EstimatedDuration { get; set; }
    public int OrderIndex { get; set; }
}
