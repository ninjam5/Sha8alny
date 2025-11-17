using Sh8lny.Application.DTOs.Certificates;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for certificate management
/// </summary>
public interface ICertificateService
{
    Task<CertificateDto> IssueCertificateAsync(IssueCertificateDto dto);
    Task<CertificateDto> GetCertificateByIdAsync(int certificateId);
    Task<CertificateDto> GetCertificateByNumberAsync(string certificateNumber);
    Task<IEnumerable<CertificateDto>> GetStudentCertificatesAsync(int studentId);
    Task<IEnumerable<CertificateDto>> GetCompanyCertificatesAsync(int companyId);
    Task<IEnumerable<CertificateDto>> GetProjectCertificatesAsync(int projectId);
    Task<CertificateDto> UpdateCertificateAsync(UpdateCertificateDto dto);
    Task<bool> DeleteCertificateAsync(int certificateId);
    Task<CertificateDto> VerifyCertificateAsync(VerifyCertificateDto dto);
}
