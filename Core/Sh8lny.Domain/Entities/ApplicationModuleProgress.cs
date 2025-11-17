namespace Sh8lny.Domain.Entities;

/// <summary>
/// Tracks completion progress for project modules per application
/// </summary>
public class ApplicationModuleProgress
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int ProjectModuleId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Application Application { get; set; } = null!;
    public ProjectModule ProjectModule { get; set; } = null!;
}
