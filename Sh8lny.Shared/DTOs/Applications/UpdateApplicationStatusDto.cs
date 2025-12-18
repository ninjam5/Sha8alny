namespace Sh8lny.Shared.DTOs.Applications;

/// <summary>
/// DTO for updating application status (company action).
/// </summary>
public class UpdateApplicationStatusDto
{
    public required string Status { get; set; }
    public string? ReviewNotes { get; set; }
}
