using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Certificates;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Service;

/// <summary>
/// Service for certificate generation and management.
/// </summary>
public class CertificateService : ICertificateService
{
    private readonly IUnitOfWork _unitOfWork;

    public CertificateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<CertificateDto>> GenerateCertificateAsync(int applicationId)
    {
        try
        {
            // 1. Verify application exists and is completed
            var application = await _unitOfWork.Applications.GetByIdAsync(applicationId);
            if (application is null)
            {
                return ServiceResponse<CertificateDto>.Failure("Application not found.");
            }

            if (application.Status != ApplicationStatus.Completed)
            {
                return ServiceResponse<CertificateDto>.Failure(
                    "Certificate can only be generated for completed applications.");
            }

            // 2. Check if certificate already exists
            var existingCert = await _unitOfWork.Certificates
                .FindSingleAsync(c => c.StudentID == application.StudentID && 
                                      c.ProjectID == application.ProjectID);
            if (existingCert is not null)
            {
                // Return existing certificate
                return await GetCertificateDtoAsync(existingCert);
            }

            // 3. Get project and company info
            var project = await _unitOfWork.Projects.GetByIdAsync(application.ProjectID);
            if (project is null)
            {
                return ServiceResponse<CertificateDto>.Failure("Project not found.");
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(project.CompanyID);
            if (company is null)
            {
                return ServiceResponse<CertificateDto>.Failure("Company not found.");
            }

            // 4. Get student info
            var student = await _unitOfWork.Students.GetByIdAsync(application.StudentID);
            if (student is null)
            {
                return ServiceResponse<CertificateDto>.Failure("Student not found.");
            }

            // 5. Generate unique certificate number (GUID-based)
            var uniqueId = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12);
            var certificateNumber = $"CERT-{uniqueId}";

            // 6. Determine project type text
            var projectTypeText = project.ProjectType?.ToString() ?? "Project";

            // 7. Create certificate
            var certificate = new Certificate
            {
                StudentID = student.StudentID,
                ProjectID = project.ProjectID,
                CompanyID = company.CompanyID,
                CertificateNumber = certificateNumber,
                CertificateTitle = $"Certificate of Completion - {projectTypeText}",
                Description = $"This certificate is awarded to {student.FullName} for successfully completing " +
                             $"the {projectTypeText.ToLower()} '{project.ProjectName}' with {company.CompanyName}.",
                CertificateURL = $"/certificates/verify/{certificateNumber}",
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = null // Certificates don't expire by default
            };

            await _unitOfWork.Certificates.AddAsync(certificate);
            await _unitOfWork.SaveAsync();

            return await GetCertificateDtoAsync(certificate);
        }
        catch (Exception ex)
        {
            return ServiceResponse<CertificateDto>.Failure(
                "An error occurred while generating the certificate.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<CertificateDto>>> GetMyCertificatesAsync(int studentUserId)
    {
        try
        {
            // 1. Get student
            var student = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == studentUserId);
            if (student is null)
            {
                return ServiceResponse<IEnumerable<CertificateDto>>.Failure("Student profile not found.");
            }

            // 2. Get all certificates for student
            var certificates = await _unitOfWork.Certificates
                .FindAsync(c => c.StudentID == student.StudentID);

            // 3. Build DTOs with related info
            var dtos = new List<CertificateDto>();
            foreach (var cert in certificates)
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(cert.ProjectID);
                var company = await _unitOfWork.Companies.GetByIdAsync(cert.CompanyID);

                dtos.Add(new CertificateDto
                {
                    Id = cert.CertificateID,
                    UniqueId = cert.CertificateNumber,
                    StudentName = student.FullName ?? "Unknown",
                    ProjectTitle = project?.ProjectName ?? "Unknown",
                    CompanyName = company?.CompanyName ?? "Unknown",
                    CertificateTitle = cert.CertificateTitle,
                    Description = cert.Description,
                    IssueDate = cert.IssuedAt,
                    ExpiresAt = cert.ExpiresAt,
                    CertificateUrl = cert.CertificateURL ?? string.Empty
                });
            }

            return ServiceResponse<IEnumerable<CertificateDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<CertificateDto>>.Failure(
                "An error occurred while retrieving certificates.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<CertificateDto>> GetCertificateByIdentifierAsync(string uniqueId)
    {
        try
        {
            // Find certificate by its unique number
            var certificate = await _unitOfWork.Certificates
                .FindSingleAsync(c => c.CertificateNumber == uniqueId);

            if (certificate is null)
            {
                return ServiceResponse<CertificateDto>.Failure("Certificate not found.");
            }

            return await GetCertificateDtoAsync(certificate);
        }
        catch (Exception ex)
        {
            return ServiceResponse<CertificateDto>.Failure(
                "An error occurred while verifying the certificate.",
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Helper method to build a CertificateDto from a Certificate entity.
    /// </summary>
    private async Task<ServiceResponse<CertificateDto>> GetCertificateDtoAsync(Certificate certificate)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(certificate.StudentID);
        var project = await _unitOfWork.Projects.GetByIdAsync(certificate.ProjectID);
        var company = await _unitOfWork.Companies.GetByIdAsync(certificate.CompanyID);

        var dto = new CertificateDto
        {
            Id = certificate.CertificateID,
            UniqueId = certificate.CertificateNumber,
            StudentName = student?.FullName ?? "Unknown",
            ProjectTitle = project?.ProjectName ?? "Unknown",
            CompanyName = company?.CompanyName ?? "Unknown",
            CertificateTitle = certificate.CertificateTitle,
            Description = certificate.Description,
            IssueDate = certificate.IssuedAt,
            ExpiresAt = certificate.ExpiresAt,
            CertificateUrl = certificate.CertificateURL ?? string.Empty
        };

        return ServiceResponse<CertificateDto>.Success(dto);
    }
}
