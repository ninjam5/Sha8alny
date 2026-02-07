namespace Sh8lny.Shared.DTOs.StudentProfile;

/// <summary>
/// DTO for work experience information.
/// </summary>
public class ExperienceDto
{
    public int? Id { get; set; }
    public required string CompanyName { get; set; }
    public required string Role { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}
