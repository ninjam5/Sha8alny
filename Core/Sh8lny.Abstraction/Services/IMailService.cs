namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for email sending service.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends an email with the specified subject and HTML body.
    /// </summary>
    /// <param name="toEmail">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="htmlBody">HTML body content.</param>
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}
