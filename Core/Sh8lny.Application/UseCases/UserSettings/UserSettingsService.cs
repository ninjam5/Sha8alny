using Sh8lny.Application.DTOs.UserSettings;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.UserSettings;

/// <summary>
/// Service for user settings and preferences
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserSettingsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserSettingsDto> GetUserSettingsAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        // Try to get existing settings
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);

        // If settings don't exist, create default settings
        if (settings == null)
        {
            settings = new Domain.Entities.UserSettings
            {
                UserID = userId,
                EmailNotifications = true,
                PushNotifications = true,
                MessageNotifications = true,
                ApplicationNotifications = true,
                Language = "en",
                Timezone = "UTC",
                ProfileVisibility = ProfileVisibility.Public,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserSettings.AddAsync(settings);
            await _unitOfWork.SaveChangesAsync();
        }

        return MapToDto(settings);
    }

    public async Task<UserSettingsDto> UpdateUserSettingsAsync(UpdateUserSettingsDto dto)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserID);

        // Get settings (will create default if not exists)
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(dto.UserID);
        
        if (settings == null)
        {
            // Create default settings first
            settings = new Domain.Entities.UserSettings
            {
                UserID = dto.UserID,
                EmailNotifications = true,
                PushNotifications = true,
                MessageNotifications = true,
                ApplicationNotifications = true,
                Language = "en",
                Timezone = "UTC",
                ProfileVisibility = ProfileVisibility.Public,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserSettings.AddAsync(settings);
            await _unitOfWork.SaveChangesAsync();
        }

        // Apply updates from DTO (only update if value is provided)
        if (dto.EmailNotifications.HasValue)
            settings.EmailNotifications = dto.EmailNotifications.Value;

        if (dto.PushNotifications.HasValue)
            settings.PushNotifications = dto.PushNotifications.Value;

        if (dto.MessageNotifications.HasValue)
            settings.MessageNotifications = dto.MessageNotifications.Value;

        if (dto.ApplicationNotifications.HasValue)
            settings.ApplicationNotifications = dto.ApplicationNotifications.Value;

        if (!string.IsNullOrWhiteSpace(dto.Language))
            settings.Language = dto.Language;

        if (!string.IsNullOrWhiteSpace(dto.Timezone))
            settings.Timezone = dto.Timezone;

        if (dto.ProfileVisibility.HasValue)
        {
            // Validate enum value
            if (!Enum.IsDefined(typeof(ProfileVisibility), dto.ProfileVisibility.Value))
                throw new ValidationException("Invalid profile visibility value. Must be 0 (Public), 1 (Private), or 2 (UniversityOnly).");

            settings.ProfileVisibility = (ProfileVisibility)dto.ProfileVisibility.Value;
        }

        settings.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.UserSettings.UpdateAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(settings);
    }

    public async Task<UserSettingsDto> UpdateNotificationPreferencesAsync(NotificationPreferencesDto dto)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserID);

        // Get settings (will create default if not exists)
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(dto.UserID);
        
        if (settings == null)
        {
            throw new NotFoundException("UserSettings", dto.UserID);
        }

        // Update notification preferences
        settings.EmailNotifications = dto.EmailNotifications;
        settings.PushNotifications = dto.PushNotifications;
        settings.MessageNotifications = dto.MessageNotifications;
        settings.ApplicationNotifications = dto.ApplicationNotifications;
        settings.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.UserSettings.UpdateAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(settings);
    }

    public async Task<UserSettingsDto> UpdatePrivacySettingsAsync(PrivacySettingsDto dto)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserID);
        if (user == null)
            throw new NotFoundException(nameof(User), dto.UserID);

        // Get settings
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(dto.UserID);
        
        if (settings == null)
        {
            throw new NotFoundException("UserSettings", dto.UserID);
        }

        // Validate enum value
        if (!Enum.IsDefined(typeof(ProfileVisibility), dto.ProfileVisibility))
            throw new ValidationException("Invalid profile visibility value. Must be 0 (Public), 1 (Private), or 2 (UniversityOnly).");

        // Update privacy settings
        settings.ProfileVisibility = (ProfileVisibility)dto.ProfileVisibility;
        settings.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.UserSettings.UpdateAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(settings);
    }

    public async Task<UserSettingsDto> CreateDefaultSettingsAsync(int userId)
    {
        // Validate user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        // Check if settings already exist
        var existingSettings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if (existingSettings != null)
            throw new ValidationException("User settings already exist for this user.");

        // Create new default settings
        var settings = new Domain.Entities.UserSettings
        {
            UserID = userId,
            EmailNotifications = true,
            PushNotifications = true,
            MessageNotifications = true,
            ApplicationNotifications = true,
            Language = "en",
            Timezone = "UTC",
            ProfileVisibility = ProfileVisibility.Public,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserSettings.AddAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(settings);
    }

    public async Task<bool> DeleteUserSettingsAsync(int userId)
    {
        // Get settings
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        
        if (settings == null)
            return false; // Nothing to delete

        await _unitOfWork.UserSettings.DeleteAsync(settings);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    #region Helper Methods

    /// <summary>
    /// Maps UserSettings entity to UserSettingsDto
    /// </summary>
    private static UserSettingsDto MapToDto(Domain.Entities.UserSettings settings)
    {
        return new UserSettingsDto
        {
            SettingID = settings.SettingID,
            UserID = settings.UserID,
            EmailNotifications = settings.EmailNotifications,
            PushNotifications = settings.PushNotifications,
            MessageNotifications = settings.MessageNotifications,
            ApplicationNotifications = settings.ApplicationNotifications,
            Language = settings.Language,
            Timezone = settings.Timezone,
            ProfileVisibility = settings.ProfileVisibility.ToString(),
            UpdatedAt = settings.UpdatedAt
        };
    }

    #endregion
}
