namespace Sh8lny.Shared.DTOs.Projects;

/// <summary>
/// DTO for filtering and searching projects.
/// </summary>
public class ProjectFilterDto
{
    /// <summary>
    /// Keyword search in Title/Description.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by project type/category.
    /// </summary>
    public string? ProjectType { get; set; }

    /// <summary>
    /// Filter by minimum duration (in days or as specified).
    /// </summary>
    public int? MinDuration { get; set; }

    /// <summary>
    /// Filter by maximum duration (in days or as specified).
    /// </summary>
    public int? MaxDuration { get; set; }

    /// <summary>
    /// Filter by required skill ID.
    /// </summary>
    public int? SkillId { get; set; }

    /// <summary>
    /// Filter by company ID.
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Filter by project status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by location/city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Only show visible projects (default: true).
    /// </summary>
    public bool? IsVisible { get; set; } = true;

    /// <summary>
    /// Sort by field (e.g., "CreatedAt", "Deadline", "ViewCount").
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort in descending order (default: true for newest first).
    /// </summary>
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Validates and normalizes filter values.
    /// </summary>
    public void Normalize()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100; // Max page size limit
    }
}
