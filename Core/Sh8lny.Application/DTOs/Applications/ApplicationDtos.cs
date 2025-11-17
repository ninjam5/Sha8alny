using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sh8lny.Application.Common;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Application.DTOs.Applications;

/// <summary>
/// Application detail DTO
/// </summary>
public class ApplicationDetailDto
{
    public int ApplicationID { get; set; }
    
    // Project info
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    
    // Student info
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentEmail { get; set; }
    public string? StudentPhone { get; set; }
    
    // Application details
    public string? CoverLetter { get; set; }
    public string Resume { get; set; } = string.Empty;
    public string? PortfolioURL { get; set; }
    public string? ProposalDocument { get; set; }
    
    // Status
    public string Status { get; set; } = string.Empty;
    
    // Review info
    public int? ReviewedBy { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    
    // Timestamps
    public DateTime AppliedAt { get; set; }

    // Progress
    public int TotalModules { get; set; }
    public int CompletedModules { get; set; }
    public double ProgressPercentage { get; set; }
    public List<int> CompletedModuleIds { get; set; } = new();
}

/// <summary>
/// Lightweight DTO representing application curriculum progress
/// </summary>
public class ApplicationProgressDto
{
    public int ApplicationId { get; set; }
    public int ProjectId { get; set; }
    public int TotalModules { get; set; }
    public int CompletedModulesCount { get; set; }
    public double ProgressPercentage { get; set; }
    public List<int> CompletedModuleIds { get; set; } = new();
}

/// <summary>
/// Application list item DTO
/// </summary>
public class ApplicationListDto
{
    public int ApplicationID { get; set; }
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? StudentProfilePicture { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}

/// <summary>
/// Submit application DTO
/// </summary>
public class SubmitApplicationDto
{
    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectID { get; set; }

    [Required(ErrorMessage = "Student ID is required")]
    public int StudentID { get; set; }

    [MaxLength(2000, ErrorMessage = "Cover Letter cannot exceed 2000 characters")]
    public string? CoverLetter { get; set; }

    [Required(ErrorMessage = "Resume is required")]
    [Url(ErrorMessage = "Invalid resume URL format")]
    [MaxLength(500, ErrorMessage = "Resume URL cannot exceed 500 characters")]
    public string Resume { get; set; } = string.Empty;

    [Url(ErrorMessage = "Invalid portfolio URL format")]
    [MaxLength(500, ErrorMessage = "Portfolio URL cannot exceed 500 characters")]
    public string? PortfolioURL { get; set; }

    [Url(ErrorMessage = "Invalid proposal document URL format")]
    [MaxLength(500, ErrorMessage = "Proposal document URL cannot exceed 500 characters")]
    public string? ProposalDocument { get; set; }
}

/// <summary>
/// Review application DTO
/// </summary>
public class ReviewApplicationDto
{
    [Required(ErrorMessage = "Application ID is required")]
    public int ApplicationID { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public ApplicationStatus Status { get; set; }

    [Required(ErrorMessage = "Reviewer ID is required")]
    public int ReviewedBy { get; set; }

    [MaxLength(1000, ErrorMessage = "Review Notes cannot exceed 1000 characters")]
    public string? ReviewNotes { get; set; }
}

/// <summary>
/// Application filter DTO
/// </summary>
public class ApplicationFilterDto : PaginationRequest
{
    public int? ProjectID { get; set; }
    public int? StudentID { get; set; }
    public int? CompanyID { get; set; }
    public ApplicationStatus? Status { get; set; }
    public DateTime? AppliedAfter { get; set; }
    public DateTime? AppliedBefore { get; set; }
}
