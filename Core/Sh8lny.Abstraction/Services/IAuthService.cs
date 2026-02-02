using Sh8lny.Shared.DTOs.Auth;
using Sh8lny.Shared.DTOs.Common;

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

    /// <summary>
    /// Gets the current authenticated user's summary information.
    /// </summary>
    /// <param name="userId">The ID of the current user.</param>
    /// <returns>User summary with display name and profile picture based on user type.</returns>
    Task<ServiceResponse<UserSummaryDto>> GetCurrentUserAsync(int userId);
}
