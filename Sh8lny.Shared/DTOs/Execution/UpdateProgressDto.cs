namespace Sh8lny.Shared.DTOs.Execution;

/// <summary>
/// DTO for updating module progress.
/// </summary>
public class UpdateProgressDto
{
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public int ProgressPercentage { get; set; }
    
    /// <summary>
    /// Optional note about the progress update.
    /// </summary>
    public string? Note { get; set; }
}
