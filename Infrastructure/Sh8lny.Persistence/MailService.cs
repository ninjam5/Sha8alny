using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.Options;

namespace Sh8lny.Persistence;

/// <summary>
/// Email service implementation using MailKit with Gmail SMTP.
/// </summary>
public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailFrom));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        try
        {
            _logger.LogInformation("Connecting to SMTP server {Host}:{Port}...", _mailSettings.SmtpHost, _mailSettings.SmtpPort);

            await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
            await smtp.SendAsync(email);

            _logger.LogInformation("Email sent successfully to {To}.", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}.", toEmail);
            throw;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
