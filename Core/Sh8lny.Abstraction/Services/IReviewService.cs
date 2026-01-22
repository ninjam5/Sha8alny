using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Reviews;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for mutual review operations between Companies and Students.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Allows a Company to review a Student after a completed job.
    /// </summary>
    /// <param name="companyUserId">The company's user ID (reviewer).</param>
    /// <param name="dto">The review data.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> ReviewStudentAsync(int companyUserId, CreateReviewDto dto);

    /// <summary>
    /// Allows a Student to review a Company after a completed job.
    /// </summary>
    /// <param name="studentUserId">The student's user ID (reviewer).</param>
    /// <param name="dto">The review data.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> ReviewCompanyAsync(int studentUserId, CreateReviewDto dto);

    /// <summary>
    /// Gets all reviews for a specific student.
    /// </summary>
    /// <param name="studentId">The student ID.</param>
    /// <returns>List of reviews for the student.</returns>
    Task<ServiceResponse<IEnumerable<ReviewResponseDto>>> GetStudentReviewsAsync(int studentId);

    /// <summary>
    /// Gets all reviews for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>List of reviews for the company.</returns>
    Task<ServiceResponse<IEnumerable<ReviewResponseDto>>> GetCompanyReviewsAsync(int companyId);
}
