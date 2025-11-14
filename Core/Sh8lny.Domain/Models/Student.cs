namespace Sh8lny.Domain.Models;

/// <summary>
/// Student profile entity
/// </summary>
public class Student
{
    // Primary key
    public int StudentID { get; set; }
    
    // Foreign key to User
    public int UserID { get; set; }

    // Personal info
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }
    public string? ProfilePicture { get; set; }

    // Academic info
    public int? UniversityID { get; set; }
    public int? DepartmentID { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    public string? StudentIDNumber { get; set; }

    // Location
    public string? City { get; set; }
    public string? State { get; set; }
    public required string Country { get; set; }

    // Profile status
    public int ProfileCompleteness { get; set; }
    public StudentStatus Status { get; set; }

    // Rating & Reviews
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public University? University { get; set; }
    public Department? Department { get; set; }
    
    // Collections
    public ICollection<StudentSkill> StudentSkills { get; set; } = new HashSet<StudentSkill>();
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new HashSet<GroupMember>();
    public ICollection<Certificate> Certificates { get; set; } = new HashSet<Certificate>();
    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public ICollection<CompletedOpportunity> CompletedOpportunities { get; set; } = new HashSet<CompletedOpportunity>();
    public ICollection<CompanyReview> CompanyReviews { get; set; } = new HashSet<CompanyReview>();
    public ICollection<StudentReview> ReceivedReviews { get; set; } = new HashSet<StudentReview>();

    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Academic year enumeration
/// </summary>
public enum AcademicYear
{
    FirstYear,
    SecondYear,
    ThirdYear,
    FourthYear,
    Graduate
}

/// <summary>
/// Student status enumeration
/// </summary>
public enum StudentStatus
{
    Active,
    Inactive,
    Suspended,
    Graduated
}
