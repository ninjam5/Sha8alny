using Sh8lny.Application.DTOs.Reviews;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for review functionality
/// </summary>
public interface IReviewService
{
    // Company Review operations
    Task<CompanyReviewDto> CreateCompanyReviewAsync(CreateCompanyReviewDto dto);
    Task<CompanyReviewDto> GetCompanyReviewByIdAsync(int reviewId);
    Task<IEnumerable<CompanyReviewDto>> GetCompanyReviewsAsync(int companyId, int page = 1, int pageSize = 20);
    Task<IEnumerable<CompanyReviewDto>> GetStudentCompanyReviewsAsync(int studentId);
    Task<CompanyReviewDto> UpdateCompanyReviewAsync(UpdateCompanyReviewDto dto, int studentId);
    Task<bool> DeleteCompanyReviewAsync(int reviewId, int studentId);
    Task<CompanyReviewDto> AddCompanyResponseAsync(CompanyResponseDto dto, int companyId);
    Task<ReviewStatsDto> GetCompanyReviewStatsAsync(int companyId);

    // Student Review operations
    Task<StudentReviewDto> CreateStudentReviewAsync(CreateStudentReviewDto dto);
    Task<StudentReviewDto> GetStudentReviewByIdAsync(int reviewId);
    Task<IEnumerable<StudentReviewDto>> GetStudentReviewsAsync(int studentId, int page = 1, int pageSize = 20);
    Task<IEnumerable<StudentReviewDto>> GetCompanyStudentReviewsAsync(int companyId);
    Task<StudentReviewDto> UpdateStudentReviewAsync(UpdateStudentReviewDto dto, int companyId);
    Task<bool> DeleteStudentReviewAsync(int reviewId, int companyId);
    Task<StudentReviewDto> AddStudentResponseAsync(StudentResponseDto dto, int studentId);
    Task<ReviewStatsDto> GetStudentReviewStatsAsync(int studentId);

    // Review moderation
    Task<bool> ApproveReviewAsync(int reviewId, string reviewType); // reviewType: "company" or "student"
    Task<bool> RejectReviewAsync(int reviewId, string reviewType);
    Task<bool> FlagReviewAsync(int reviewId, string reviewType, int reportingUserId);
}
