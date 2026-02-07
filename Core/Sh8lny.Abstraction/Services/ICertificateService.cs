using Sh8lny.Shared.DTOs.Certificates;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for certificate operations.
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Generates a certificate for a completed application/job.
    /// </summary>
    /// <param name="applicationId">The application ID.</param>
    /// <returns>Service response containing the generated certificate.</returns>
    Task<ServiceResponse<CertificateDto>> GenerateCertificateAsync(int applicationId);

    /// <summary>
    /// Gets all certificates for a student.
    /// </summary>
    /// <param name="studentUserId">The student's user ID.</param>
    /// <returns>Service response containing the list of certificates.</returns>
    Task<ServiceResponse<IEnumerable<CertificateDto>>> GetMyCertificatesAsync(int studentUserId);

    /// <summary>
    /// Gets a certificate by its unique identifier (for public verification).
    /// </summary>
    /// <param name="uniqueId">The unique certificate identifier.</param>
    /// <returns>Service response containing the certificate details.</returns>
    Task<ServiceResponse<CertificateDto>> GetCertificateByIdentifierAsync(string uniqueId);
}
