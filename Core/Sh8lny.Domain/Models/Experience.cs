namespace Sh8lny.Domain.Models;

/// <summary>
/// Represents a student's work experience.
/// </summary>
public class Experience
{
    // Primary key
    public int ExperienceID { get; set; }

    // Foreign key
    public int StudentID { get; set; }

    // Experience details
    public required string CompanyName { get; set; }
    public required string Role { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Student Student { get; set; } = null!;
}
