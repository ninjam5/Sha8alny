using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Students;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Student management service interface
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Get student profile by ID
    /// </summary>
    Task<ApiResponse<StudentProfileDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get student profile by user ID
    /// </summary>
    Task<ApiResponse<StudentProfileDto>> GetStudentByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated list of students with filters
    /// </summary>
    Task<ApiResponse<PagedResult<StudentListDto>>> GetStudentsAsync(StudentFilterDto filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new student profile
    /// </summary>
    Task<ApiResponse<StudentProfileDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update student profile
    /// </summary>
    Task<ApiResponse<StudentProfileDto>> UpdateStudentAsync(int studentId, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete student profile
    /// </summary>
    Task<ApiResponse<bool>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add skill to student
    /// </summary>
    Task<ApiResponse<bool>> AddSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove skill from student
    /// </summary>
    Task<ApiResponse<bool>> RemoveSkillAsync(int studentId, int skillId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get student's skills
    /// </summary>
    Task<ApiResponse<List<SkillDto>>> GetStudentSkillsAsync(int studentId, CancellationToken cancellationToken = default);
}
