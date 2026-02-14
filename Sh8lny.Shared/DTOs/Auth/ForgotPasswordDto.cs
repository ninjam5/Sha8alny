namespace Sh8lny.Shared.DTOs.Auth;

/// <summary>
/// DTO for the forgot-password request.
/// </summary>
public class ForgotPasswordDto
{
    public required string Email { get; set; }
}
