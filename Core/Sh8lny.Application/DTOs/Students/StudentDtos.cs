using System.ComponentModel.DataAnnotations;
using Sh8lny.Application.Common;

namespace Sh8lny.Application.DTOs.Students;

/// <summary>
/// Student profile view DTO
/// </summary>
public class StudentProfileDto
{
    public int StudentID { get; set; }
    public int UserID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    
    // Academic info
    public int? UniversityID { get; set; }
    public string? UniversityName { get; set; }
    public int? DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public string? AcademicYear { get; set; }
    public decimal? GPA { get; set; }
    
    // Experience
    public string? ResumeURL { get; set; }
    public string? PortfolioURL { get; set; }
    public string? LinkedInProfile { get; set; }
    public string? GitHubProfile { get; set; }
    
    // Stats
    public int TotalApplications { get; set; }
    public int AcceptedApplications { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    
    // Skills
    public List<SkillDto> Skills { get; set; } = new();
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Student list item DTO (for search/browse)
/// </summary>
public class StudentListDto
{
    public int StudentID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? UniversityName { get; set; }
    public string? DepartmentName { get; set; }
    public string? AcademicYear { get; set; }
    public decimal? GPA { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<string> Skills { get; set; } = new();
}

/// <summary>
/// Create student DTO
/// </summary>
public class CreateStudentDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }

    [MaxLength(50, ErrorMessage = "Academic Year cannot exceed 50 characters")]
    public string? AcademicYear { get; set; }

    [Range(0, 5, ErrorMessage = "GPA must be between 0 and 5")]
    public decimal? GPA { get; set; }

    [MaxLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
    public string? Bio { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Invalid resume URL format")]
    [MaxLength(500, ErrorMessage = "Resume URL cannot exceed 500 characters")]
    public string? ResumeURL { get; set; }

    [Url(ErrorMessage = "Invalid portfolio URL format")]
    [MaxLength(500, ErrorMessage = "Portfolio URL cannot exceed 500 characters")]
    public string? PortfolioURL { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [MaxLength(500, ErrorMessage = "LinkedIn profile URL cannot exceed 500 characters")]
    public string? LinkedInProfile { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL format")]
    [MaxLength(500, ErrorMessage = "GitHub profile URL cannot exceed 500 characters")]
    public string? GitHubProfile { get; set; }
}

/// <summary>
/// Update student profile DTO
/// </summary>
public class UpdateStudentDto
{
    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }

    [MaxLength(50, ErrorMessage = "Academic Year cannot exceed 50 characters")]
    public string? AcademicYear { get; set; }

    [Range(0, 5, ErrorMessage = "GPA must be between 0 and 5")]
    public decimal? GPA { get; set; }

    [MaxLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
    public string? Bio { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Invalid profile picture URL format")]
    [MaxLength(500, ErrorMessage = "Profile picture URL cannot exceed 500 characters")]
    public string? ProfilePicture { get; set; }

    [Url(ErrorMessage = "Invalid resume URL format")]
    [MaxLength(500, ErrorMessage = "Resume URL cannot exceed 500 characters")]
    public string? ResumeURL { get; set; }

    [Url(ErrorMessage = "Invalid portfolio URL format")]
    [MaxLength(500, ErrorMessage = "Portfolio URL cannot exceed 500 characters")]
    public string? PortfolioURL { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [MaxLength(500, ErrorMessage = "LinkedIn profile URL cannot exceed 500 characters")]
    public string? LinkedInProfile { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL format")]
    [MaxLength(500, ErrorMessage = "GitHub profile URL cannot exceed 500 characters")]
    public string? GitHubProfile { get; set; }
}

/// <summary>
/// Student skill DTO
/// </summary>
public class SkillDto
{
    public int SkillID { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
}

/// <summary>
/// Add skill to student DTO
/// </summary>
public class AddStudentSkillDto
{
    [Required(ErrorMessage = "Student ID is required")]
    public int StudentID { get; set; }

    [Required(ErrorMessage = "Skill ID is required")]
    public int SkillID { get; set; }
}

/// <summary>
/// Student search/filter DTO
/// </summary>
public class StudentFilterDto : PaginationRequest
{
    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }
    public string? AcademicYear { get; set; }
    public int? SkillID { get; set; }
    public decimal? MinGPA { get; set; }
    public decimal? MinRating { get; set; }
    public string? SearchTerm { get; set; }
}
