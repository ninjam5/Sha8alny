using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.Certificates;

#region Certificate DTOs

/// <summary>
/// DTO for issuing a new certificate
/// </summary>
public class IssueCertificateDto
{
    [Required(ErrorMessage = "Student ID is required")]
    public int StudentID { get; set; }

    [Required(ErrorMessage = "Project ID is required")]
    public int ProjectID { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public int CompanyID { get; set; }

    [Required(ErrorMessage = "Certificate number is required")]
    [MaxLength(50, ErrorMessage = "Certificate number cannot exceed 50 characters")]
    public string CertificateNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Certificate title is required")]
    [MaxLength(200, ErrorMessage = "Certificate title cannot exceed 200 characters")]
    public string CertificateTitle { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? CertificateURL { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// DTO for certificate details
/// </summary>
public class CertificateDto
{
    public int CertificateID { get; set; }
    public int StudentID { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CertificateURL { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
}

/// <summary>
/// DTO for updating certificate details
/// </summary>
public class UpdateCertificateDto
{
    [Required(ErrorMessage = "Certificate ID is required")]
    public int CertificateID { get; set; }

    [MaxLength(200, ErrorMessage = "Certificate title cannot exceed 200 characters")]
    public string? CertificateTitle { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid URL format")]
    public string? CertificateURL { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// DTO for verifying a certificate by certificate number
/// </summary>
public class VerifyCertificateDto
{
    [Required(ErrorMessage = "Certificate number is required")]
    [MaxLength(50, ErrorMessage = "Certificate number cannot exceed 50 characters")]
    public string CertificateNumber { get; set; } = string.Empty;
}

#endregion
