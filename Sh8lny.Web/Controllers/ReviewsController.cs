using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Reviews;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for mutual review operations between Companies and Students.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Allows a Company to review a Student after a completed job.
    /// </summary>
    /// <param name="dto">The review data.</param>
    /// <returns>Success or failure response.</returns>
    [HttpPost("student")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<bool>>> ReviewStudent([FromBody] CreateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _reviewService.ReviewStudentAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Allows a Student to review a Company after a completed job.
    /// </summary>
    /// <param name="dto">The review data.</param>
    /// <returns>Success or failure response.</returns>
    [HttpPost("company")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ServiceResponse<bool>>> ReviewCompany([FromBody] CreateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _reviewService.ReviewCompanyAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all reviews for a specific student.
    /// </summary>
    /// <param name="studentId">The student ID.</param>
    /// <returns>List of reviews for the student.</returns>
    [HttpGet("student/{studentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ReviewResponseDto>>>> GetStudentReviews(int studentId)
    {
        var result = await _reviewService.GetStudentReviewsAsync(studentId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all reviews for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>List of reviews for the company.</returns>
    [HttpGet("company/{companyId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ReviewResponseDto>>>> GetCompanyReviews(int companyId)
    {
        var result = await _reviewService.GetCompanyReviewsAsync(companyId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Extracts the current user ID from JWT claims.
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }
        return userId;
    }
}
