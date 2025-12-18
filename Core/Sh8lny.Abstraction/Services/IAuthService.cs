using Sh8lny.Shared.DTOs.Auth;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">Registration data.</param>
    /// <returns>Authentication response with token if successful.</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="dto">Login credentials.</param>
    /// <returns>Authentication response with token if successful.</returns>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
