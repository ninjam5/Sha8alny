using System.Collections.Generic;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Projects;
using Sh8lny.Domain.Entities;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Project/Opportunity management service interface
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Get project by ID
    /// </summary>
    Task<ApiResponse<ProjectDetailDto>> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get paginated list of projects with filters
    /// </summary>
    Task<ApiResponse<PagedResult<ProjectListDto>>> GetProjectsAsync(ProjectFilterDto filter, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get active projects
    /// </summary>
    Task<ApiResponse<List<ProjectListDto>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get projects by company
    /// </summary>
    Task<ApiResponse<List<ProjectListDto>>> GetProjectsByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Search projects by term
    /// </summary>
    Task<ApiResponse<List<ProjectListDto>>> SearchProjectsAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new project
    /// </summary>
    Task<ApiResponse<ProjectDetailDto>> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update project
    /// </summary>
    Task<ApiResponse<ProjectDetailDto>> UpdateProjectAsync(int projectId, UpdateProjectDto dto, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete project
    /// </summary>
    Task<ApiResponse<bool>> DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update project status
    /// </summary>
    Task<ApiResponse<bool>> UpdateProjectStatusAsync(int projectId, ProjectStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Increment project view count
    /// </summary>
    Task<ApiResponse<bool>> IncrementViewCountAsync(int projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a curriculum module to a project
    /// </summary>
    Task<ApiResponse<ProjectModuleDto>> AddModuleAsync(int projectId, CreateProjectModuleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a curriculum module and reindex remaining modules
    /// </summary>
    Task<ApiResponse<bool>> DeleteModuleAsync(int moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persist a new order for modules
    /// </summary>
    Task<ApiResponse<bool>> ReorderModulesAsync(int projectId, List<int> moduleIdsInNewOrder, CancellationToken cancellationToken = default);
}
