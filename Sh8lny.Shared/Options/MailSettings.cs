namespace Sh8lny.Shared.Options;

/// <summary>
/// Configuration options for email (SMTP) settings.
/// </summary>
public class MailSettings
{
    public const string SectionName = "MailSettings";

    public required string EmailFrom { get; set; }
    public required string DisplayName { get; set; }
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public required string SmtpUser { get; set; }
    public required string SmtpPass { get; set; }
}
