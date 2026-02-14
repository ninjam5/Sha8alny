namespace Sh8lny.Shared.DTOs.Projects;

/// <summary>
/// DTO for advanced searching, filtering, and sorting of projects.
/// All properties are optional — omit a filter to skip it.
/// </summary>
public class ProjectFilterDto
{
    // ── Text Search ────────────────────────────────────────

    /// <summary>
    /// Keyword search — matches against Title and Description.
    /// </summary>
    public string? Keyword { get; set; }

    // ── Filters ────────────────────────────────────────────

    /// <summary>
    /// Filter by project type (e.g., "Internship", "FullTime").
    /// </summary>
    public string? ProjectType { get; set; }

    /// <summary>
    /// Filter by project status (e.g., "Active", "Closed").
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by company ID.
    /// </summary>
    public int? CompanyId { get; set; }

    /// <summary>
    /// Only return projects whose deadline is before this date.
    /// </summary>
    public DateTime? DeadlineBefore { get; set; }

    /// <summary>
    /// Only return projects whose deadline is after this date.
    /// </summary>
    public DateTime? DeadlineAfter { get; set; }

    /// <summary>
    /// Filter by required skill IDs.
    /// If provided, projects must require at least one of these skills.
    /// </summary>
    public List<int>? SkillIds { get; set; }

    /// <summary>
    /// Only show visible projects (default: true).
    /// </summary>
    public bool? IsVisible { get; set; } = true;

    // ── Sorting ────────────────────────────────────────────

    /// <summary>
    /// Sort preset. Supported values:
    /// "newest" (default), "oldest", "deadline_asc", "deadline_desc",
    /// "views_desc", "applications_desc", "title_asc", "title_desc".
    /// </summary>
    public string? SortBy { get; set; }

    // ── Pagination ─────────────────────────────────────────

    /// <summary>
    /// Page number (1-based, default: 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default: 10, max: 100).
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Validates and normalizes filter values.
    /// </summary>
    public void Normalize()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;

        Keyword = Keyword?.Trim();
        SortBy = SortBy?.Trim().ToLowerInvariant();
    }
}
