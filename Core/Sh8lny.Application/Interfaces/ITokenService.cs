namespace Sh8lny.Application.Interfaces;

/// <summary>
/// JWT token service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate access token (JWT)
    /// </summary>
    string GenerateAccessToken(int userId, string email, string userType);
    
    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Validate token
    /// </summary>
    bool ValidateToken(string token);
    
    /// <summary>
    /// Extract user ID from token
    /// </summary>
    int? GetUserIdFromToken(string token);
}
