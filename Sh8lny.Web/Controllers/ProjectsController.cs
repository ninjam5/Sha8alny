using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Projects;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProjectById(int id, CancellationToken cancellationToken)
    {
        // Increment view count
        await _projectService.IncrementViewCountAsync(id, cancellationToken);
        
        var result = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of projects with filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectsAsync(filter, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get active projects
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveProjects(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetActiveProjectsAsync(cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get projects by company
    /// </summary>
    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetProjectsByCompany(int companyId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectsByCompanyAsync(companyId, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Search projects
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProjects([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        var result = await _projectService.SearchProjectsAsync(searchTerm, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create new project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.CreateProjectAsync(dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetProjectById), new { id = result.Data?.ProjectID }, result);
    }

    /// <summary>
    /// Update project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateProjectAsync(id, dto, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete project
    /// </summary>
    [Authorize(Roles = "Company,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id, CancellationToken cancellationToken)
    {
        var result = await _projectService.DeleteProjectAsync(id, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Update project status
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateProjectStatus(int id, [FromBody] UpdateProjectStatusDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateProjectStatusAsync(id, dto.Status, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Add a curriculum module to a project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPost("{projectId}/modules")]
    public async Task<IActionResult> AddModule(int projectId, [FromBody] CreateProjectModuleDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.AddModuleAsync(projectId, dto, cancellationToken);

        if (!result.Success)
            return BuildFailureResponse(result);

        return CreatedAtAction(nameof(GetProjectById), new { id = projectId }, result);
    }

    /// <summary>
    /// Delete a module from a project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpDelete("modules/{moduleId}")]
    public async Task<IActionResult> DeleteModule(int moduleId, CancellationToken cancellationToken)
    {
        var result = await _projectService.DeleteModuleAsync(moduleId, cancellationToken);

        if (!result.Success)
            return BuildFailureResponse(result);

        return Ok(result);
    }

    /// <summary>
    /// Reorder modules for a project
    /// </summary>
    [Authorize(Roles = "Company")]
    [HttpPut("{projectId}/modules/reorder")]
    public async Task<IActionResult> ReorderModules(int projectId, [FromBody] ReorderProjectModulesDto dto, CancellationToken cancellationToken)
    {
        if (dto?.ModuleIds == null || dto.ModuleIds.Count == 0)
            return BadRequest(ApiResponse<bool>.FailureResponse("Module order payload is required"));

        var result = await _projectService.ReorderModulesAsync(projectId, dto.ModuleIds, cancellationToken);

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
