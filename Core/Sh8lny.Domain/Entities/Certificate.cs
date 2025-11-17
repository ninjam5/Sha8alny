namespace Sh8lny.Domain.Entities;

/// <summary>
/// Certificate entity for student achievements
/// </summary>
public class Certificate
{
    // Primary key
    public int CertificateID { get; set; }

    // Foreign keys
    public int StudentID { get; set; }
    public int ProjectID { get; set; }
    public int CompanyID { get; set; }

    // Certificate details
    public required string CertificateNumber { get; set; }
    public required string CertificateTitle { get; set; }
    public string? Description { get; set; }
    public string? CertificateURL { get; set; }

    /* Verification
    public bool IsVerified { get; set; }
    public int? VerifiedBy { get; set; }
    */

    // Timestamps
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public CompletedOpportunity? CompletedOpportunity { get; set; }
}
