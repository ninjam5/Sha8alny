namespace Sh8lny.Shared.DTOs.Auth;

/// <summary>
/// DTO for returning current user's basic information for navigation/profile display.
/// </summary>
public class UserSummaryDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
}
