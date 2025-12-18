namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// DTO for module progress details.
/// </summary>
public class ModuleProgressDto
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Summary DTO for application progress across all modules.
/// </summary>
public class ProjectProgressDto
{
    public int ApplicationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ApplicationStatus { get; set; } = string.Empty;
    
    /// <summary>
    /// Overall weighted progress percentage.
    /// </summary>
    public decimal OverallProgress { get; set; }
    
    /// <summary>
    /// Progress details for each module.
    /// </summary>
    public List<ModuleProgressDto> Modules { get; set; } = new();
    
    /// <summary>
    /// Count of completed modules.
    /// </summary>
    public int CompletedModules { get; set; }
    
    /// <summary>
    /// Total number of modules.
    /// </summary>
    public int TotalModules { get; set; }
}
