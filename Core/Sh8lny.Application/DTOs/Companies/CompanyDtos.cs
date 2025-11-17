using System.ComponentModel.DataAnnotations;
using Sh8lny.Application.Common;

namespace Sh8lny.Application.DTOs.Companies;

/// <summary>
/// Company profile view DTO
/// </summary>
public class CompanyProfileDto
{
    public int CompanyID { get; set; }
    public int UserID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? LogoURL { get; set; }
    public string? Description { get; set; }
    
    // Company info
    public string? Industry { get; set; }
    public string? CompanySize { get; set; }
    public string? FoundedYear { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    
    // Social
    public string? LinkedInProfile { get; set; }
    public string? TwitterProfile { get; set; }
    public string? FacebookProfile { get; set; }
    
    // Stats
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalHires { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    
    // Status
    public bool IsVerified { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Company list item DTO (for search/browse)
/// </summary>
public class CompanyListDto
{
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LogoURL { get; set; }
    public string? Industry { get; set; }
    public string? City { get; set; }
    public int ActiveProjects { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsVerified { get; set; }
}

/// <summary>
/// Create company DTO
/// </summary>
public class CreateCompanyDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    [MaxLength(100, ErrorMessage = "Industry cannot exceed 100 characters")]
    public string? Industry { get; set; }

    [MaxLength(50, ErrorMessage = "Company Size cannot exceed 50 characters")]
    public string? CompanySize { get; set; }

    [MaxLength(4, ErrorMessage = "Founded Year must be 4 digits")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Founded Year must be a valid 4-digit year")]
    public string? FoundedYear { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    public string? Website { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }
}

/// <summary>
/// Update company profile DTO
/// </summary>
public class UpdateCompanyDto
{
    [MaxLength(100, ErrorMessage = "Industry cannot exceed 100 characters")]
    public string? Industry { get; set; }

    [MaxLength(50, ErrorMessage = "Company Size cannot exceed 50 characters")]
    public string? CompanySize { get; set; }

    [MaxLength(4, ErrorMessage = "Founded Year must be 4 digits")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Founded Year must be a valid 4-digit year")]
    public string? FoundedYear { get; set; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid website URL format")]
    [MaxLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
    public string? Website { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    [Url(ErrorMessage = "Invalid logo URL format")]
    [MaxLength(500, ErrorMessage = "Logo URL cannot exceed 500 characters")]
    public string? LogoURL { get; set; }

    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [MaxLength(500, ErrorMessage = "LinkedIn profile URL cannot exceed 500 characters")]
    public string? LinkedInProfile { get; set; }

    [Url(ErrorMessage = "Invalid Twitter URL format")]
    [MaxLength(500, ErrorMessage = "Twitter profile URL cannot exceed 500 characters")]
    public string? TwitterProfile { get; set; }

    [Url(ErrorMessage = "Invalid Facebook URL format")]
    [MaxLength(500, ErrorMessage = "Facebook profile URL cannot exceed 500 characters")]
    public string? FacebookProfile { get; set; }
}

/// <summary>
/// Company search/filter DTO
/// </summary>
public class CompanyFilterDto : PaginationRequest
{
    public string? Industry { get; set; }
    public string? City { get; set; }
    public bool? IsVerified { get; set; }
    public decimal? MinRating { get; set; }
    public string? SearchTerm { get; set; }
}
