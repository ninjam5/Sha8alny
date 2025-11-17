using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.Reviews;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Interfaces;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, IUnitOfWork unitOfWork, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets the StudentID from the authenticated user's JWT token
    /// </summary>
    private async Task<int> GetAuthenticatedStudentIdAsync()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
        return student?.StudentID ?? 0;
    }
    
    /// <summary>
    /// Gets the CompanyID from the authenticated user's JWT token
    /// </summary>
    private async Task<int> GetAuthenticatedCompanyIdAsync()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var company = await _unitOfWork.Companies.GetByUserIdAsync(userId);
        return company?.CompanyID ?? 0;
    }

    #region Company Review Endpoints

    [HttpPost("companies")]
    public async Task<IActionResult> CreateCompanyReview([FromBody] CreateCompanyReviewDto dto)
    {
        var result = await _reviewService.CreateCompanyReviewAsync(dto);
        return CreatedAtAction(nameof(GetCompanyReview), new { id = result.ReviewID }, result);
    }

    [HttpGet("companies/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyReview(int id)
    {
        var result = await _reviewService.GetCompanyReviewByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("companies/by-company/{companyId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyReviews(int companyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetCompanyReviewsAsync(companyId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("companies/by-student/{studentId}")]
    public async Task<IActionResult> GetStudentCompanyReviews(int studentId)
    {
        var result = await _reviewService.GetStudentCompanyReviewsAsync(studentId);
        return Ok(result);
    }

    [HttpPut("companies/{id}")]
    public async Task<IActionResult> UpdateCompanyReview(int id, [FromBody] UpdateCompanyReviewDto dto)
    {
        var studentId = await GetAuthenticatedStudentIdAsync();
        var result = await _reviewService.UpdateCompanyReviewAsync(dto, studentId);
        return Ok(result);
    }

    [HttpDelete("companies/{id}")]
    public async Task<IActionResult> DeleteCompanyReview(int id)
    {
        var studentId = await GetAuthenticatedStudentIdAsync();
        await _reviewService.DeleteCompanyReviewAsync(id, studentId);
        return NoContent();
    }

    [HttpPost("companies/{id}/response")]
    public async Task<IActionResult> AddCompanyResponse(int id, [FromBody] CompanyResponseDto dto)
    {
        var companyId = await GetAuthenticatedCompanyIdAsync();
        var result = await _reviewService.AddCompanyResponseAsync(dto, companyId);
        return Ok(result);
    }

    [HttpGet("companies/stats/{companyId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyReviewStats(int companyId)
    {
        var result = await _reviewService.GetCompanyReviewStatsAsync(companyId);
        return Ok(result);
    }

    #endregion

    #region Student Review Endpoints

    [HttpPost("students")]
    public async Task<IActionResult> CreateStudentReview([FromBody] CreateStudentReviewDto dto)
    {
        var result = await _reviewService.CreateStudentReviewAsync(dto);
        return CreatedAtAction(nameof(GetStudentReview), new { id = result.ReviewID }, result);
    }

    [HttpGet("students/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentReview(int id)
    {
        var result = await _reviewService.GetStudentReviewByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("students/by-student/{studentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentReviews(int studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetStudentReviewsAsync(studentId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("students/by-company/{companyId}")]
    public async Task<IActionResult> GetCompanyStudentReviews(int companyId)
    {
        var result = await _reviewService.GetCompanyStudentReviewsAsync(companyId);
        return Ok(result);
    }

    [HttpPut("students/{id}")]
    public async Task<IActionResult> UpdateStudentReview(int id, [FromBody] UpdateStudentReviewDto dto)
    {
        var companyId = await GetAuthenticatedCompanyIdAsync();
        var result = await _reviewService.UpdateStudentReviewAsync(dto, companyId);
        return Ok(result);
    }

    [HttpDelete("students/{id}")]
    public async Task<IActionResult> DeleteStudentReview(int id)
    {
        var companyId = await GetAuthenticatedCompanyIdAsync();
        await _reviewService.DeleteStudentReviewAsync(id, companyId);
        return NoContent();
    }

    [HttpPost("students/{id}/response")]
    public async Task<IActionResult> AddStudentResponse(int id, [FromBody] StudentResponseDto dto)
    {
        var studentId = await GetAuthenticatedStudentIdAsync();
        var result = await _reviewService.AddStudentResponseAsync(dto, studentId);
        return Ok(result);
    }

    [HttpGet("students/stats/{studentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStudentReviewStats(int studentId)
    {
        var result = await _reviewService.GetStudentReviewStatsAsync(studentId);
        return Ok(result);
    }

    #endregion

    #region Review Moderation (Admin)

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveReview(int id, [FromQuery] string reviewType)
    {
        await _reviewService.ApproveReviewAsync(id, reviewType);
        return Ok();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectReview(int id, [FromQuery] string reviewType)
    {
        await _reviewService.RejectReviewAsync(id, reviewType);
        return Ok();
    }

    [HttpPost("{id}/flag")]
    public async Task<IActionResult> FlagReview(int id, [FromQuery] string reviewType)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _reviewService.FlagReviewAsync(id, reviewType, userId);
        return Ok();
    }

    #endregion
}
