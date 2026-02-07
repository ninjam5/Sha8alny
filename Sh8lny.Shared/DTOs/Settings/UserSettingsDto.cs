namespace Sh8lny.Shared.DTOs.Settings;

/// <summary>
/// DTO for user settings.
/// </summary>
public class UserSettingsDto
{
    /// <summary>
    /// The user ID these settings belong to.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Whether email notifications are enabled.
    /// </summary>
    public bool EnableEmailNotifications { get; set; }

    /// <summary>
    /// Whether push notifications are enabled.
    /// </summary>
    public bool EnablePushNotifications { get; set; }

    /// <summary>
    /// Whether message notifications are enabled.
    /// </summary>
    public bool EnableMessageNotifications { get; set; }

    /// <summary>
    /// Whether application-related notifications are enabled.
    /// </summary>
    public bool EnableApplicationNotifications { get; set; }

    /// <summary>
    /// Preferred language (e.g., "en", "ar").
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Preferred timezone (e.g., "Africa/Cairo", "UTC").
    /// </summary>
    public string Timezone { get; set; } = "UTC";

    /// <summary>
    /// Profile visibility setting.
    /// </summary>
    public string ProfileVisibility { get; set; } = "Public";
}
