namespace Sh8lny.Shared.DTOs.Auth;

/// <summary>
/// DTO for resetting a user's password using the 6-digit code.
/// </summary>
public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}
