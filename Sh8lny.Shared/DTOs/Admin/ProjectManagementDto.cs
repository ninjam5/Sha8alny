namespace Sh8lny.Shared.DTOs.Admin;

/// <summary>
/// DTO for project management in admin panel.
/// </summary>
public class ProjectManagementDto
{
    /// <summary>
    /// Project ID.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Project name.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Company name that owns the project.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Project status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of applications for this project.
    /// </summary>
    public int ApplicationCount { get; set; }

    /// <summary>
    /// When the project was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Whether the project is visible to applicants.
    /// </summary>
    public bool IsVisible { get; set; }
}
