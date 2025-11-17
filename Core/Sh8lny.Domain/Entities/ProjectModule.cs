namespace Sh8lny.Domain.Entities;

/// <summary>
/// Defines a structured module within a project curriculum
/// </summary>
public class ProjectModule
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? EstimatedDuration { get; set; }
    public int OrderIndex { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<ApplicationModuleProgress> ModuleProgress { get; set; } = new HashSet<ApplicationModuleProgress>();
}
