using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Applications;
using Sh8lny.Application.Interfaces;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IApplicationService applicationService, ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Get application by ID
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationById(int id, CancellationToken cancellationToken)
    {
        var result = await _applicationService.GetApplicationByIdAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of applications with filters
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _applicationService.GetApplicationsAsync(filter, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get applications by project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetApplicationsByProject(int projectId, CancellationToken cancellationToken)
    {
        var result = await _applicationService.GetApplicationsByProjectAsync(projectId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get applications by student
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetApplicationsByStudent(int studentId, CancellationToken cancellationToken)
    {
        var result = await _applicationService.GetApplicationsByStudentAsync(studentId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Submit new application
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> SubmitApplication([FromBody] SubmitApplicationDto dto, CancellationToken cancellationToken)
    {
        var result = await _applicationService.SubmitApplicationAsync(dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetApplicationById), new { id = result.Data?.ApplicationID }, result);
    }

    /// <summary>
    /// Review application (accept/reject)
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}/review")]
    public async Task<IActionResult> ReviewApplication(int id, [FromBody] ReviewApplicationDto dto, CancellationToken cancellationToken)
    {
        var result = await _applicationService.ReviewApplicationAsync(dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Withdraw application
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpDelete("{id}/withdraw")]
    public async Task<IActionResult> WithdrawApplication(int id, [FromQuery] int studentId, CancellationToken cancellationToken)
    {
        var result = await _applicationService.WithdrawApplicationAsync(id, studentId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Check if student already applied to project
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpGet("check")]
    public async Task<IActionResult> HasStudentApplied([FromQuery] int projectId, [FromQuery] int studentId, CancellationToken cancellationToken)
    {
        var result = await _applicationService.HasStudentAppliedAsync(projectId, studentId, cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Toggle module completion for an application
    /// </summary>
    [Authorize(Roles = "Student")]
    [HttpPost("{applicationId}/modules/{moduleId}/toggle")]
    public async Task<IActionResult> ToggleModuleCompletion(int applicationId, int moduleId, [FromBody] ToggleModuleCompletionRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return BadRequest(ApiResponse<ApplicationProgressDto>.FailureResponse("Request body is required"));

        var result = await _applicationService.ToggleModuleCompletionAsync(applicationId, moduleId, request.IsCompleted, cancellationToken);

        if (!result.Success)
            return BuildFailureResponse(result);

        return Ok(result);
    }

    private IActionResult BuildFailureResponse<T>(ApiResponse<T> result)
    {
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            if (result.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(result);

            if (result.Message.Contains("not authorized", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(result);
        }

        return BadRequest(result);
    }
}

public class ToggleModuleCompletionRequest
{
    public bool IsCompleted { get; set; }
}
