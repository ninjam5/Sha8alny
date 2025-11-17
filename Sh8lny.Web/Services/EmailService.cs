using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string verificationCode, CancellationToken cancellationToken = default)
    {
        var subject = "Verify Your Email - Sha8alny Platform";
        var body = $@"
            <h2>Welcome to Sha8alny!</h2>
            <p>Thank you for registering. Please verify your email address by using the following code:</p>
            <h3 style='color: #4CAF50; letter-spacing: 2px;'>{verificationCode}</h3>
            <p>This code will expire in 24 hours.</p>
            <p>If you didn't create an account, please ignore this email.</p>
            <br/>
            <p>Best regards,<br/>Sha8alny Team</p>
        ";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetCode, CancellationToken cancellationToken = default)
    {
        var subject = "Password Reset Request - Sha8alny Platform";
        var body = $@"
            <h2>Password Reset</h2>
            <p>You requested to reset your password. Use the following code to reset your password:</p>
            <h3 style='color: #FF5722; letter-spacing: 2px;'>{resetCode}</h3>
            <p>This code will expire in 1 hour.</p>
            <p>If you didn't request a password reset, please ignore this email and ensure your account is secure.</p>
            <br/>
            <p>Best regards,<br/>Sha8alny Team</p>
        ";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending using SendGrid, SMTP, or other email service
        // For now, just log the email
        
        _logger.LogInformation("=== EMAIL SENT ===");
        _logger.LogInformation("To: {ToEmail}", toEmail);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: {Body}", body);
        _logger.LogInformation("==================");

        // Simulate async operation
        await Task.Delay(100, cancellationToken);

        // In production, implement with SendGrid:
        /*
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], "Sha8alny");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
        await client.SendEmailAsync(msg, cancellationToken);
        */
    }

    public async Task SendApplicationStatusEmailAsync(string toEmail, string projectName, string status, CancellationToken cancellationToken = default)
    {
        var subject = $"Application Status Update - {projectName}";
        var body = $@"
            <h2>Application Status Update</h2>
            <p>Your application for <strong>{projectName}</strong> has been updated.</p>
            <p>Status: <strong style='color: #4CAF50;'>{status}</strong></p>
            <br/>
            <p>Best regards,<br/>Sha8alny Team</p>
        ";

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }
}
