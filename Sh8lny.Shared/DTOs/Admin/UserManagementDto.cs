namespace Sh8lny.Shared.DTOs.Admin;

/// <summary>
/// DTO for user management in admin panel.
/// </summary>
public class UserManagementDto
{
    /// <summary>
    /// User ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's display name (student name or company name).
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// User role (Student, Company, Admin).
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user account is active (not banned).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the user's email is verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// When the user registered.
    /// </summary>
    public DateTime JoinDate { get; set; }

    /// <summary>
    /// When the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// User's profile picture URL.
    /// </summary>
    public string? ProfilePicture { get; set; }
}
