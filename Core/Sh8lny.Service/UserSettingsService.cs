using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Settings;

namespace Sh8lny.Service;

/// <summary>
/// Service for user settings operations.
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserSettingsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<UserSettingsDto>> GetSettingsAsync(int userId)
    {
        try
        {
            // 1. Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<UserSettingsDto>.Failure("User not found.");
            }

            // 2. Try to get existing settings
            var settings = await _unitOfWork.UserSettings.FindSingleAsync(s => s.UserID == userId);

            // 3. If settings don't exist, create default settings
            if (settings is null)
            {
                settings = new UserSettings
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
                await _unitOfWork.SaveAsync();
            }

            // 4. Map to DTO and return
            var dto = MapToDto(settings);
            return ServiceResponse<UserSettingsDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ServiceResponse<UserSettingsDto>.Failure(
                "An error occurred while retrieving settings.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<UserSettingsDto>> UpdateSettingsAsync(int userId, UserSettingsDto dto)
    {
        try
        {
            // 1. Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<UserSettingsDto>.Failure("User not found.");
            }

            // 2. Get existing settings (or create if they don't exist)
            var settings = await _unitOfWork.UserSettings.FindSingleAsync(s => s.UserID == userId);

            if (settings is null)
            {
                // Create new settings with provided values
                settings = new UserSettings
                {
                    UserID = userId,
                    EmailNotifications = dto.EnableEmailNotifications,
                    PushNotifications = dto.EnablePushNotifications,
                    MessageNotifications = dto.EnableMessageNotifications,
                    ApplicationNotifications = dto.EnableApplicationNotifications,
                    Language = dto.Language,
                    Timezone = dto.Timezone,
                    ProfileVisibility = ParseProfileVisibility(dto.ProfileVisibility),
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserSettings.AddAsync(settings);
            }
            else
            {
                // Update existing settings
                settings.EmailNotifications = dto.EnableEmailNotifications;
                settings.PushNotifications = dto.EnablePushNotifications;
                settings.MessageNotifications = dto.EnableMessageNotifications;
                settings.ApplicationNotifications = dto.EnableApplicationNotifications;
                settings.Language = dto.Language;
                settings.Timezone = dto.Timezone;
                settings.ProfileVisibility = ParseProfileVisibility(dto.ProfileVisibility);
                settings.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.UserSettings.Update(settings);
            }

            await _unitOfWork.SaveAsync();

            // 3. Return updated settings
            var resultDto = MapToDto(settings);
            return ServiceResponse<UserSettingsDto>.Success(resultDto, "Settings updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<UserSettingsDto>.Failure(
                "An error occurred while updating settings.",
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Maps a UserSettings entity to a DTO.
    /// </summary>
    private static UserSettingsDto MapToDto(UserSettings settings)
    {
        return new UserSettingsDto
        {
            UserId = settings.UserID,
            EnableEmailNotifications = settings.EmailNotifications,
            EnablePushNotifications = settings.PushNotifications,
            EnableMessageNotifications = settings.MessageNotifications,
            EnableApplicationNotifications = settings.ApplicationNotifications,
            Language = settings.Language,
            Timezone = settings.Timezone,
            ProfileVisibility = settings.ProfileVisibility.ToString()
        };
    }

    /// <summary>
    /// Parses a string to ProfileVisibility enum.
    /// </summary>
    private static ProfileVisibility ParseProfileVisibility(string visibility)
    {
        return visibility?.ToLower() switch
        {
            "private" => ProfileVisibility.Private,
            "universityonly" => ProfileVisibility.UniversityOnly,
            _ => ProfileVisibility.Public
        };
    }
}
