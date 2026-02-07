namespace Sh8lny.Shared.DTOs.Certificates;

/// <summary>
/// DTO representing a certificate.
/// </summary>
public class CertificateDto
{
    /// <summary>
    /// Certificate ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier for public verification.
    /// </summary>
    public string UniqueId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the student who received the certificate.
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Title of the project/internship completed.
    /// </summary>
    public string ProjectTitle { get; set; } = string.Empty;

    /// <summary>
    /// Name of the company that issued the certificate.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Certificate title/description.
    /// </summary>
    public string CertificateTitle { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of achievements.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When the certificate was issued.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// When the certificate expires (if applicable).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Public verification URL.
    /// </summary>
    public string CertificateUrl { get; set; } = string.Empty;
}
