using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.Reviews;

#region Company Review DTOs

/// <summary>
/// DTO for creating a company review
/// </summary>
public class CreateCompanyReviewDto
{
    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyID { get; set; }

    [Required(ErrorMessage = "Student ID is required")]
    public int StudentID { get; set; }

    public int? CompletedOpportunityID { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public decimal Rating { get; set; }

    [MaxLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    // Detailed ratings (optional)
    [Range(1, 5, ErrorMessage = "Work environment rating must be between 1 and 5")]
    public decimal? WorkEnvironmentRating { get; set; }

    [Range(1, 5, ErrorMessage = "Learning opportunity rating must be between 1 and 5")]
    public decimal? LearningOpportunityRating { get; set; }

    [Range(1, 5, ErrorMessage = "Mentorship rating must be between 1 and 5")]
    public decimal? MentorshipRating { get; set; }

    [Range(1, 5, ErrorMessage = "Compensation rating must be between 1 and 5")]
    public decimal? CompensationRating { get; set; }

    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public decimal? CommunicationRating { get; set; }

    [Required(ErrorMessage = "Would recommend field is required")]
    public bool WouldRecommend { get; set; }

    [MaxLength(1000, ErrorMessage = "Pros cannot exceed 1000 characters")]
    public string? Pros { get; set; }

    [MaxLength(1000, ErrorMessage = "Cons cannot exceed 1000 characters")]
    public string? Cons { get; set; }

    public bool IsAnonymous { get; set; }
}

/// <summary>
/// DTO for company review details
/// </summary>
public class CompanyReviewDto
{
    public int ReviewID { get; set; }
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int StudentID { get; set; }
    public string? StudentName { get; set; } // Null if anonymous
    public int? CompletedOpportunityID { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewTitle { get; set; }
    public string? ReviewText { get; set; }
    public decimal? WorkEnvironmentRating { get; set; }
    public decimal? LearningOpportunityRating { get; set; }
    public decimal? MentorshipRating { get; set; }
    public decimal? CompensationRating { get; set; }
    public decimal? CommunicationRating { get; set; }
    public bool WouldRecommend { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CompanyResponse { get; set; }
    public DateTime? CompanyRespondedAt { get; set; }
}

/// <summary>
/// DTO for updating a company review
/// </summary>
public class UpdateCompanyReviewDto
{
    [Required(ErrorMessage = "Review ID is required")]
    public int ReviewID { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public decimal? Rating { get; set; }

    [MaxLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    [Range(1, 5, ErrorMessage = "Work environment rating must be between 1 and 5")]
    public decimal? WorkEnvironmentRating { get; set; }

    [Range(1, 5, ErrorMessage = "Learning opportunity rating must be between 1 and 5")]
    public decimal? LearningOpportunityRating { get; set; }

    [Range(1, 5, ErrorMessage = "Mentorship rating must be between 1 and 5")]
    public decimal? MentorshipRating { get; set; }

    [Range(1, 5, ErrorMessage = "Compensation rating must be between 1 and 5")]
    public decimal? CompensationRating { get; set; }

    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public decimal? CommunicationRating { get; set; }

    public bool? WouldRecommend { get; set; }

    [MaxLength(1000, ErrorMessage = "Pros cannot exceed 1000 characters")]
    public string? Pros { get; set; }

    [MaxLength(1000, ErrorMessage = "Cons cannot exceed 1000 characters")]
    public string? Cons { get; set; }
}

/// <summary>
/// DTO for company response to a review
/// </summary>
public class CompanyResponseDto
{
    [Required(ErrorMessage = "Review ID is required")]
    public int ReviewID { get; set; }

    [Required(ErrorMessage = "Company response is required")]
    [MaxLength(1000, ErrorMessage = "Company response cannot exceed 1000 characters")]
    public string CompanyResponse { get; set; } = string.Empty;
}

#endregion

#region Student Review DTOs

/// <summary>
/// DTO for creating a student review
/// </summary>
public class CreateStudentReviewDto
{
    [Required(ErrorMessage = "Student ID is required")]
    public int StudentID { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyID { get; set; }

    public int? CompletedOpportunityID { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public decimal Rating { get; set; }

    [MaxLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    // Detailed ratings (optional)
    [Range(1, 5, ErrorMessage = "Technical skills rating must be between 1 and 5")]
    public decimal? TechnicalSkillsRating { get; set; }

    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public decimal? CommunicationRating { get; set; }

    [Range(1, 5, ErrorMessage = "Professionalism rating must be between 1 and 5")]
    public decimal? ProfessionalismRating { get; set; }

    [Range(1, 5, ErrorMessage = "Time management rating must be between 1 and 5")]
    public decimal? TimeManagementRating { get; set; }

    [Range(1, 5, ErrorMessage = "Teamwork rating must be between 1 and 5")]
    public decimal? TeamworkRating { get; set; }

    [Range(1, 5, ErrorMessage = "Problem solving rating must be between 1 and 5")]
    public decimal? ProblemSolvingRating { get; set; }

    [Required(ErrorMessage = "Would hire again field is required")]
    public bool WouldHireAgain { get; set; }

    [MaxLength(1000, ErrorMessage = "Strengths cannot exceed 1000 characters")]
    public string? Strengths { get; set; }

    [MaxLength(1000, ErrorMessage = "Areas for improvement cannot exceed 1000 characters")]
    public string? AreasForImprovement { get; set; }

    public bool IsPublic { get; set; }
}

/// <summary>
/// DTO for student review details
/// </summary>
public class StudentReviewDto
{
    public int ReviewID { get; set; }
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int? CompletedOpportunityID { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewTitle { get; set; }
    public string? ReviewText { get; set; }
    public decimal? TechnicalSkillsRating { get; set; }
    public decimal? CommunicationRating { get; set; }
    public decimal? ProfessionalismRating { get; set; }
    public decimal? TimeManagementRating { get; set; }
    public decimal? TeamworkRating { get; set; }
    public decimal? ProblemSolvingRating { get; set; }
    public bool WouldHireAgain { get; set; }
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? StudentResponse { get; set; }
    public DateTime? StudentRespondedAt { get; set; }
}

/// <summary>
/// DTO for updating a student review
/// </summary>
public class UpdateStudentReviewDto
{
    [Required(ErrorMessage = "Review ID is required")]
    public int ReviewID { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public decimal? Rating { get; set; }

    [MaxLength(200, ErrorMessage = "Review title cannot exceed 200 characters")]
    public string? ReviewTitle { get; set; }

    [MaxLength(2000, ErrorMessage = "Review text cannot exceed 2000 characters")]
    public string? ReviewText { get; set; }

    [Range(1, 5, ErrorMessage = "Technical skills rating must be between 1 and 5")]
    public decimal? TechnicalSkillsRating { get; set; }

    [Range(1, 5, ErrorMessage = "Communication rating must be between 1 and 5")]
    public decimal? CommunicationRating { get; set; }

    [Range(1, 5, ErrorMessage = "Professionalism rating must be between 1 and 5")]
    public decimal? ProfessionalismRating { get; set; }

    [Range(1, 5, ErrorMessage = "Time management rating must be between 1 and 5")]
    public decimal? TimeManagementRating { get; set; }

    [Range(1, 5, ErrorMessage = "Teamwork rating must be between 1 and 5")]
    public decimal? TeamworkRating { get; set; }

    [Range(1, 5, ErrorMessage = "Problem solving rating must be between 1 and 5")]
    public decimal? ProblemSolvingRating { get; set; }

    public bool? WouldHireAgain { get; set; }

    [MaxLength(1000, ErrorMessage = "Strengths cannot exceed 1000 characters")]
    public string? Strengths { get; set; }

    [MaxLength(1000, ErrorMessage = "Areas for improvement cannot exceed 1000 characters")]
    public string? AreasForImprovement { get; set; }

    public bool? IsPublic { get; set; }
}

/// <summary>
/// DTO for student response to a review
/// </summary>
public class StudentResponseDto
{
    [Required(ErrorMessage = "Review ID is required")]
    public int ReviewID { get; set; }

    [Required(ErrorMessage = "Student response is required")]
    [MaxLength(1000, ErrorMessage = "Student response cannot exceed 1000 characters")]
    public string StudentResponse { get; set; } = string.Empty;
}

#endregion

#region Common Review DTOs

/// <summary>
/// DTO for review statistics
/// </summary>
public class ReviewStatsDto
{
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
    public int RecommendationPercentage { get; set; }
}

#endregion
