using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Applications;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Application management service interface
/// </summary>
public interface IApplicationService
{
    /// <summary>
    /// Get application by ID
    /// </summary>
    Task<ApiResponse<ApplicationDetailDto>> GetApplicationByIdAsync(int applicationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated list of applications with filters
    /// </summary>
    Task<ApiResponse<PagedResult<ApplicationListDto>>> GetApplicationsAsync(ApplicationFilterDto filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get applications by project
    /// </summary>
    Task<ApiResponse<List<ApplicationListDto>>> GetApplicationsByProjectAsync(int projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get applications by student
    /// </summary>
    Task<ApiResponse<List<ApplicationListDto>>> GetApplicationsByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Submit new application
    /// </summary>
    Task<ApiResponse<ApplicationDetailDto>> SubmitApplicationAsync(SubmitApplicationDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Review application (accept/reject)
    /// </summary>
    Task<ApiResponse<ApplicationDetailDto>> ReviewApplicationAsync(ReviewApplicationDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Withdraw application (by student)
    /// </summary>
    Task<ApiResponse<bool>> WithdrawApplicationAsync(int applicationId, int studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if student already applied to project
    /// </summary>
    Task<ApiResponse<bool>> HasStudentAppliedAsync(int projectId, int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle completion state of a project module for an application
    /// </summary>
    Task<ApiResponse<ApplicationProgressDto>> ToggleModuleCompletionAsync(int applicationId, int moduleId, bool isCompleted, CancellationToken cancellationToken = default);
}
