namespace Sh8lny.Domain.Models;

/// <summary>
/// Represents a student's educational background.
/// </summary>
public class Education
{
    // Primary key
    public int EducationID { get; set; }

    // Foreign key
    public int StudentID { get; set; }

    // Education details
    public required string UniversityName { get; set; }
    public required string Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public string? Description { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Student Student { get; set; } = null!;
}
