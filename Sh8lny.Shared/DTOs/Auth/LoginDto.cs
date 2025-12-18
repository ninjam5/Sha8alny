namespace Sh8lny.Shared.DTOs.Auth;

/// <summary>
/// DTO for user login.
/// </summary>
public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
