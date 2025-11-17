using System.ComponentModel.DataAnnotations;
using Sh8lny.Application.Common;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Application.DTOs.Projects;

/// <summary>
/// Project detail view DTO
/// </summary>
public class ProjectDetailDto
{
    public int ProjectID { get; set; }
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogo { get; set; }
    
    // Project details
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ProjectType { get; set; }
    
    // Timeline
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime Deadline { get; set; }
    public string? Duration { get; set; }
    
    // Requirements
    public List<string> RequiredSkills { get; set; } = new();
    public List<ProjectModuleDto> Modules { get; set; } = new();
    public string? MinAcademicYear { get; set; }
    public int? MaxApplicants { get; set; }
    
    // Compensation
    public string? CompensationType { get; set; }
    public decimal? CompensationAmount { get; set; }
    public string? Currency { get; set; }
    
    // Status
    public string Status { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    
    // Metrics
    public int ViewCount { get; set; }
    public int ApplicationCount { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Project list item DTO (for search/browse)
/// </summary>
public class ProjectListDto
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyLogo { get; set; }
    public string? ProjectType { get; set; }
    public DateTime Deadline { get; set; }
    public List<string> RequiredSkills { get; set; } = new();
    public string? CompensationType { get; set; }
    public decimal? CompensationAmount { get; set; }
    public int ApplicationCount { get; set; }
    public int? MaxApplicants { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create project DTO
/// </summary>
public class CreateProjectDto
{
    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyID { get; set; }

    [Required(ErrorMessage = "Project Name is required")]
    [MinLength(3, ErrorMessage = "Project Name must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Project Name cannot exceed 200 characters")]
    public string ProjectName { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Project Code cannot exceed 50 characters")]
    public string? ProjectCode { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [MinLength(50, ErrorMessage = "Description must be at least 50 characters")]
    [MaxLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Project Type cannot exceed 50 characters")]
    public string? ProjectType { get; set; }
    
    // Timeline
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Deadline is required")]
    public DateTime Deadline { get; set; }

    [MaxLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
    public string? Duration { get; set; }
    
    // Requirements
    public List<int>? RequiredSkillIDs { get; set; }

    [MaxLength(50, ErrorMessage = "Minimum Academic Year cannot exceed 50 characters")]
    public string? MinAcademicYear { get; set; }

    [Range(1, 1000, ErrorMessage = "Max Applicants must be between 1 and 1000")]
    public int? MaxApplicants { get; set; }
    
    // Compensation
    [MaxLength(50, ErrorMessage = "Compensation Type cannot exceed 50 characters")]
    public string? CompensationType { get; set; }

    [Range(0, 1000000, ErrorMessage = "Compensation Amount must be between 0 and 1,000,000")]
    public decimal? CompensationAmount { get; set; }

    [MaxLength(10, ErrorMessage = "Currency cannot exceed 10 characters")]
    public string? Currency { get; set; }
    
    // Additional
    [MaxLength(2000, ErrorMessage = "Benefits cannot exceed 2000 characters")]
    public string? Benefits { get; set; }

    [MaxLength(2000, ErrorMessage = "Application Instructions cannot exceed 2000 characters")]
    public string? ApplicationInstructions { get; set; }
}

/// <summary>
/// Update project DTO
/// </summary>
public class UpdateProjectDto
{
    [MinLength(3, ErrorMessage = "Project Name must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Project Name cannot exceed 200 characters")]
    public string? ProjectName { get; set; }

    [MinLength(50, ErrorMessage = "Description must be at least 50 characters")]
    [MaxLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string? Description { get; set; }

    [MaxLength(50, ErrorMessage = "Project Type cannot exceed 50 characters")]
    public string? ProjectType { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? Deadline { get; set; }

    [MaxLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
    public string? Duration { get; set; }

    public List<int>? RequiredSkillIDs { get; set; }

    [MaxLength(50, ErrorMessage = "Minimum Academic Year cannot exceed 50 characters")]
    public string? MinAcademicYear { get; set; }

    [Range(1, 1000, ErrorMessage = "Max Applicants must be between 1 and 1000")]
    public int? MaxApplicants { get; set; }

    [MaxLength(50, ErrorMessage = "Compensation Type cannot exceed 50 characters")]
    public string? CompensationType { get; set; }

    [Range(0, 1000000, ErrorMessage = "Compensation Amount must be between 0 and 1,000,000")]
    public decimal? CompensationAmount { get; set; }

    [MaxLength(10, ErrorMessage = "Currency cannot exceed 10 characters")]
    public string? Currency { get; set; }

    [MaxLength(2000, ErrorMessage = "Benefits cannot exceed 2000 characters")]
    public string? Benefits { get; set; }

    [MaxLength(2000, ErrorMessage = "Application Instructions cannot exceed 2000 characters")]
    public string? ApplicationInstructions { get; set; }

    public bool? IsVisible { get; set; }
}

/// <summary>
/// Project search/filter DTO
/// </summary>
public class ProjectFilterDto : PaginationRequest
{
    public int? CompanyID { get; set; }
    public string? ProjectType { get; set; }
    public string? Status { get; set; }
    public int? SkillID { get; set; }
    public string? CompensationType { get; set; }
    public decimal? MinCompensation { get; set; }
    public DateTime? DeadlineAfter { get; set; }
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Project module view DTO
/// </summary>
public class ProjectModuleDto
{
    public int ProjectModuleId { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Duration { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// Create project module DTO
/// </summary>
public class CreateProjectModuleDto
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [MaxLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
    public string? Duration { get; set; }

    [Range(1, 100, ErrorMessage = "Order index must be between 1 and 100")]
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Reorder project modules DTO
/// </summary>
public class ReorderProjectModulesDto
{
    [Required(ErrorMessage = "Module order is required")]
    public List<int> ModuleIds { get; set; } = new();
}

/// <summary>
/// Update project status DTO
/// </summary>
public class UpdateProjectStatusDto
{
    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectID { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public ProjectStatus Status { get; set; }
}
