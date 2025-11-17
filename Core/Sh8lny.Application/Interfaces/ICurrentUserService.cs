namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Current user service interface for accessing HttpContext user
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get current user ID from JWT claims
    /// </summary>
    int? UserId { get; }
    
    /// <summary>
    /// Get current user email from JWT claims
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Get current user type from JWT claims
    /// </summary>
    string? UserType { get; }
    
    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
