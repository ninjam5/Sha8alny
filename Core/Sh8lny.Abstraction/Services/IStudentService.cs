using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.StudentProfile;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for student profile operations.
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Creates a complete student profile with education, experience, and skills.
    /// </summary>
    /// <param name="userId">The ID of the user creating the profile.</param>
    /// <param name="dto">The profile data.</param>
    /// <returns>Service response containing the created student ID.</returns>
    Task<ServiceResponse<int>> CreateProfileAsync(int userId, CreateStudentProfileDto dto);
}
