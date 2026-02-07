using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Settings;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for user settings operations.
/// </summary>
public interface IUserSettingsService
{
    /// <summary>
    /// Gets settings for a user. Creates default settings if they don't exist.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Service response containing the user settings.</returns>
    Task<ServiceResponse<UserSettingsDto>> GetSettingsAsync(int userId);

    /// <summary>
    /// Updates settings for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="dto">The settings to update.</param>
    /// <returns>Service response containing the updated settings.</returns>
    Task<ServiceResponse<UserSettingsDto>> UpdateSettingsAsync(int userId, UserSettingsDto dto);
}
