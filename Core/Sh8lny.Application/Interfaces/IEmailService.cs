namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send verification email with code
    /// </summary>
    Task SendVerificationEmailAsync(string toEmail, string verificationCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send password reset email with code
    /// </summary>
    Task SendPasswordResetEmailAsync(string toEmail, string resetCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send generic email
    /// </summary>
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send application status notification email
    /// </summary>
    Task SendApplicationStatusEmailAsync(string toEmail, string projectName, string status, CancellationToken cancellationToken = default);
}
