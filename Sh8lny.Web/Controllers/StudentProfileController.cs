using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.StudentProfile;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for student profile management.
/// </summary>
[ApiController]
[Route("api/student")]
[Authorize]
public class StudentProfileController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentProfileController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Creates a complete student profile with education, experience, and skills.
    /// </summary>
    /// <param name="dto">The profile data.</param>
    /// <returns>The created student ID.</returns>
    [HttpPost("profile")]
    public async Task<ActionResult<ServiceResponse<int>>> CreateProfile([FromBody] CreateStudentProfileDto dto)
    {
        // Extract UserId from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ServiceResponse<int>.Failure("Invalid or missing user token."));
        }

        var result = await _studentService.CreateProfileAsync(userId, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
