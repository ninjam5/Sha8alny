using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Projects;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for project/opportunity management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Creates a new project/opportunity.
    /// </summary>
    /// <param name="dto">The project data.</param>
    /// <returns>The created project ID.</returns>
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<int>>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<int>.Failure("Invalid or missing user token."));
        }

        var result = await _projectService.CreateProjectAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetProjectById), new { id = result.Data }, result);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="dto">The updated project data.</param>
    /// <returns>Success or failure response.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _projectService.UpdateProjectAsync(userId.Value, id, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>Success or failure response.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteProject(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _projectService.DeleteProjectAsync(userId.Value, id);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>The project details.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<ProjectResponseDto>>> GetProjectById(int id)
    {
        var result = await _projectService.GetProjectByIdAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all projects for the current company.
    /// </summary>
    /// <returns>List of company projects.</returns>
    [HttpGet("my-projects")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ProjectResponseDto>>>> GetMyProjects()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<IEnumerable<ProjectResponseDto>>.Failure("Invalid or missing user token."));
        }

        var result = await _projectService.GetCompanyProjectsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Searches and filters projects with pagination.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>Paginated list of matching projects.</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<PagedResult<ProjectResponseDto>>>> SearchProjects(
        [FromQuery] ProjectFilterDto filter)
    {
        var result = await _projectService.GetFilteredProjectsAsync(filter);

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
