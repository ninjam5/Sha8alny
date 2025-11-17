using Sh8lny.Application.DTOs.Certificates;
using Sh8lny.Application.DTOs.Notifications;
using Sh8lny.Application.DTOs.ActivityLogs;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Certificates;

/// <summary>
/// Service for certificate management
/// </summary>
public class CertificateService : ICertificateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IActivityLogService _activityLogService;

    public CertificateService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService, IActivityLogService activityLogService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _activityLogService = activityLogService;
    }

    public async Task<CertificateDto> IssueCertificateAsync(IssueCertificateDto dto)
    {
        // Get authenticated user (must be a company)
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
            throw new UnauthorizedException("User must be authenticated to issue certificates.");

        var companyProfile = await _unitOfWork.Companies.GetByUserIdAsync(currentUserId.Value);
        if (companyProfile == null)
            throw new UnauthorizedException("Only companies can issue certificates.");

        // Validate that the project belongs to this company
        var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectID);
        if (project == null)
            throw new NotFoundException("Project", dto.ProjectID);

        if (project.CompanyID != companyProfile.CompanyID)
            throw new UnauthorizedException("You can only issue certificates for your own projects.");

        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(dto.StudentID);
        if (student == null)
            throw new NotFoundException("Student", dto.StudentID);

        // Generate unique certificate number using GUID
        var certificateNumber = Guid.NewGuid().ToString();

        // Create certificate entity
        var certificate = new Certificate
        {
            StudentID = dto.StudentID,
            ProjectID = dto.ProjectID,
            CompanyID = companyProfile.CompanyID,
            CertificateNumber = certificateNumber,
            CertificateTitle = dto.CertificateTitle,
            Description = dto.Description,
            CertificateURL = dto.CertificateURL,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = dto.ExpiresAt
        };

        await _unitOfWork.Certificates.AddAsync(certificate);
        await _unitOfWork.SaveChangesAsync();

        // Reload with related entities
        var savedCertificate = await _unitOfWork.Certificates.GetByCertificateNumberAsync(certificateNumber);

        // Log company's action
        await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
        {
            UserID = companyProfile.UserID,
            ActivityType = "CertificateIssued",
            Description = $"Company {companyProfile.CompanyName} issued certificate to student {student.FirstName} {student.LastName}",
            RelatedEntityType = "Certificate",
            RelatedEntityID = certificate.CertificateID
        });

        // Send notification to student
        try
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserID = student.UserID,
                NotificationType = (int)NotificationType.Certificate,
                Title = "Certificate Issued",
                Message = $"Congratulations! {companyProfile.CompanyName} has issued you a certificate for completing: {project.ProjectName}",
                ActionURL = $"/profile/certificates/{certificate.CertificateID}"
            });
        }
        catch
        {
            // Silently fail notification - don't break certificate issuance
        }

        return MapToCertificateDto(savedCertificate!);
    }

    public async Task<CertificateDto> GetCertificateByIdAsync(int certificateId)
    {
        var certificate = await _unitOfWork.Certificates.GetByIdAsync(certificateId);
        if (certificate == null)
            throw new NotFoundException("Certificate", certificateId);

        // Reload with related entities
        var fullCertificate = await _unitOfWork.Certificates.GetByCertificateNumberAsync(certificate.CertificateNumber);
        return MapToCertificateDto(fullCertificate!);
    }

    public async Task<CertificateDto> GetCertificateByNumberAsync(string certificateNumber)
    {
        var certificate = await _unitOfWork.Certificates.GetByCertificateNumberAsync(certificateNumber);
        if (certificate == null)
            throw new NotFoundException("Certificate with number", certificateNumber);

        return MapToCertificateDto(certificate);
    }

    public async Task<IEnumerable<CertificateDto>> GetStudentCertificatesAsync(int studentId)
    {
        // Public method - no authorization required
        var certificates = await _unitOfWork.Certificates.GetByStudentIdAsync(studentId);
        return certificates.Select(MapToCertificateDto);
    }

    public async Task<IEnumerable<CertificateDto>> GetCompanyCertificatesAsync(int companyId)
    {
        var certificates = await _unitOfWork.Certificates.GetByCompanyIdAsync(companyId);
        return certificates.Select(MapToCertificateDto);
    }

    public async Task<IEnumerable<CertificateDto>> GetProjectCertificatesAsync(int projectId)
    {
        var allCertificates = await _unitOfWork.Certificates.GetAllAsync();
        var projectCertificates = allCertificates.Where(c => c.ProjectID == projectId);
        
        // Load related entities for each certificate
        var result = new List<CertificateDto>();
        foreach (var cert in projectCertificates)
        {
            var fullCert = await _unitOfWork.Certificates.GetByCertificateNumberAsync(cert.CertificateNumber);
            if (fullCert != null)
                result.Add(MapToCertificateDto(fullCert));
        }
        
        return result;
    }

    public async Task<CertificateDto> UpdateCertificateAsync(UpdateCertificateDto dto)
    {
        // Get authenticated user (must be a company)
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
            throw new UnauthorizedException("User must be authenticated to update certificates.");

        var companyProfile = await _unitOfWork.Companies.GetByUserIdAsync(currentUserId.Value);
        if (companyProfile == null)
            throw new UnauthorizedException("Only companies can update certificates.");

        var certificate = await _unitOfWork.Certificates.GetByIdAsync(dto.CertificateID);
        if (certificate == null)
            throw new NotFoundException("Certificate", dto.CertificateID);

        // Authorize: only the issuing company can update
        if (certificate.CompanyID != companyProfile.CompanyID)
            throw new UnauthorizedException("You can only update certificates issued by your company.");

        // Update fields (selective update)
        if (!string.IsNullOrEmpty(dto.CertificateTitle))
            certificate.CertificateTitle = dto.CertificateTitle;
        
        if (dto.Description != null)
            certificate.Description = dto.Description;
        
        if (dto.CertificateURL != null)
            certificate.CertificateURL = dto.CertificateURL;
        
        if (dto.ExpiresAt.HasValue)
            certificate.ExpiresAt = dto.ExpiresAt.Value;

        await _unitOfWork.Certificates.UpdateAsync(certificate);
        await _unitOfWork.SaveChangesAsync();

        // Reload with related entities
        var updatedCertificate = await _unitOfWork.Certificates.GetByCertificateNumberAsync(certificate.CertificateNumber);
        return MapToCertificateDto(updatedCertificate!);
    }

    public async Task<bool> DeleteCertificateAsync(int certificateId)
    {
        // Get authenticated user (must be a company)
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null)
            throw new UnauthorizedException("User must be authenticated to delete certificates.");

        var companyProfile = await _unitOfWork.Companies.GetByUserIdAsync(currentUserId.Value);
        if (companyProfile == null)
            throw new UnauthorizedException("Only companies can delete certificates.");

        var certificate = await _unitOfWork.Certificates.GetByIdAsync(certificateId);
        if (certificate == null)
            throw new NotFoundException("Certificate", certificateId);

        // Authorize: only the issuing company can delete
        if (certificate.CompanyID != companyProfile.CompanyID)
            throw new UnauthorizedException("You can only delete certificates issued by your company.");

        await _unitOfWork.Certificates.DeleteAsync(certificate);
        await _unitOfWork.SaveChangesAsync();

        // Log company's action
        await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
        {
            UserID = companyProfile.UserID,
            ActivityType = "CertificateRevoked",
            Description = $"Company {companyProfile.CompanyName} revoked certificate {certificateId}",
            RelatedEntityType = "Certificate",
            RelatedEntityID = certificateId
        });

        return true;
    }

    public async Task<CertificateDto> VerifyCertificateAsync(VerifyCertificateDto dto)
    {
        // Public verification method - no authentication required
        var certificate = await _unitOfWork.Certificates.GetByCertificateNumberAsync(dto.CertificateNumber);
        if (certificate == null)
            throw new NotFoundException("Certificate with number", dto.CertificateNumber);

        return MapToCertificateDto(certificate);
    }

    private CertificateDto MapToCertificateDto(Certificate certificate)
    {
        return new CertificateDto
        {
            CertificateID = certificate.CertificateID,
            StudentID = certificate.StudentID,
            ProjectID = certificate.ProjectID,
            CompanyID = certificate.CompanyID,
            CertificateNumber = certificate.CertificateNumber,
            CertificateTitle = certificate.CertificateTitle,
            Description = certificate.Description,
            CertificateURL = certificate.CertificateURL,
            IssuedAt = certificate.IssuedAt,
            ExpiresAt = certificate.ExpiresAt,
            StudentName = certificate.Student?.FullName ?? "Unknown",
            ProjectName = certificate.Project?.ProjectName ?? "Unknown",
            CompanyName = certificate.Company?.CompanyName ?? "Unknown",
            IsExpired = certificate.ExpiresAt.HasValue && certificate.ExpiresAt.Value < DateTime.UtcNow
        };
    }
}
