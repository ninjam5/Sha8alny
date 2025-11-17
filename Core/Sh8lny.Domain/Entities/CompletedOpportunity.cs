namespace Sh8lny.Domain.Entities;

/// <summary>
/// Completed opportunity entity tracking finished internships, freelance jobs, and training programs
/// </summary>
public class CompletedOpportunity
{
    // Primary key
    public int CompletedOpportunityID { get; set; }

    // Foreign keys
    public int StudentID { get; set; }
    public int ProjectID { get; set; }
    public int? ApplicationID { get; set; }
    public int? CertificateID { get; set; }

    // Completion details
    public required string OpportunityTitle { get; set; }
    public string? Description { get; set; }
    public OpportunityType OpportunityType { get; set; }

    // Timeline
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationInDays { get; set; }

    // Performance & Feedback
    public decimal? Rating { get; set; }
    public string? StudentFeedback { get; set; }
    public string? CompanyFeedback { get; set; }
    public string? Achievements { get; set; }

    // Skills gained
    public string? SkillsGained { get; set; }

    // Status
    public CompletionStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public int? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Payment info (if applicable)
    public bool IsPaid { get; set; }
    public decimal? TotalPayment { get; set; }

    // Visibility
    public bool IsVisibleOnProfile { get; set; }

    // Timestamps
    public DateTime CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Application? Application { get; set; }
    public Certificate? Certificate { get; set; }
    public User? Verifier { get; set; }
    public CompanyReview? CompanyReview { get; set; }
    public StudentReview? StudentReview { get; set; }
}

/// <summary>
/// Opportunity type enumeration
/// </summary>
public enum OpportunityType
{
    Internship,
    FreelanceJob,
    Training,
    GraduationProject,
    PartTimeJob,
    FullTimeJob,
    Volunteering,
    Workshop
}

/// <summary>
/// Completion status enumeration
/// </summary>
public enum CompletionStatus
{
    Completed,
    PartiallyCompleted,
    Terminated,
    OnHold,
    UnderReview
}
