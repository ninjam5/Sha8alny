using Sh8lny.Application.DTOs.UserSettings;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for user settings and preferences
/// </summary>
public interface IUserSettingsService
{
    Task<UserSettingsDto> GetUserSettingsAsync(int userId);
    Task<UserSettingsDto> UpdateUserSettingsAsync(UpdateUserSettingsDto dto);
    Task<UserSettingsDto> UpdateNotificationPreferencesAsync(NotificationPreferencesDto dto);
    Task<UserSettingsDto> UpdatePrivacySettingsAsync(PrivacySettingsDto dto);
    Task<UserSettingsDto> CreateDefaultSettingsAsync(int userId);
    Task<bool> DeleteUserSettingsAsync(int userId);
}
