using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.UserSettings;

#region User Settings DTOs

/// <summary>
/// DTO for user settings details
/// </summary>
public class UserSettingsDto
{
    public int SettingID { get; set; }
    public int UserID { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public bool MessageNotifications { get; set; }
    public bool ApplicationNotifications { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public string ProfileVisibility { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for updating user settings
/// </summary>
public class UpdateUserSettingsDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    public bool? EmailNotifications { get; set; }
    public bool? PushNotifications { get; set; }
    public bool? MessageNotifications { get; set; }
    public bool? ApplicationNotifications { get; set; }

    [MaxLength(10, ErrorMessage = "Language code cannot exceed 10 characters")]
    public string? Language { get; set; }

    [MaxLength(50, ErrorMessage = "Timezone cannot exceed 50 characters")]
    public string? Timezone { get; set; }

    public int? ProfileVisibility { get; set; } // 0 = Public, 1 = Private, 2 = UniversityOnly
}

/// <summary>
/// DTO for notification preferences
/// </summary>
public class NotificationPreferencesDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    [Required]
    public bool EmailNotifications { get; set; }

    [Required]
    public bool PushNotifications { get; set; }

    [Required]
    public bool MessageNotifications { get; set; }

    [Required]
    public bool ApplicationNotifications { get; set; }
}

/// <summary>
/// DTO for privacy settings
/// </summary>
public class PrivacySettingsDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }

    [Required(ErrorMessage = "Profile visibility is required")]
    public int ProfileVisibility { get; set; } // 0 = Public, 1 = Private, 2 = UniversityOnly
}

#endregion
