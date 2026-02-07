namespace Sh8lny.Shared.DTOs.StudentProfile;

/// <summary>
/// DTO for education information.
/// </summary>
public class EducationDto
{
    public int? Id { get; set; }
    public required string UniversityName { get; set; }
    public required string Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public string? Description { get; set; }
}
