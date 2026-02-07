using Sh8lny.Shared.DTOs.Common;
using Sh8lny.Shared.DTOs.Projects;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for project/opportunity operations.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Creates a new project for the company.
    /// </summary>
    /// <param name="userId">The ID of the user (must be a Company).</param>
    /// <param name="dto">The project data.</param>
    /// <returns>Service response containing the created project ID.</returns>
    Task<ServiceResponse<int>> CreateProjectAsync(int userId, CreateProjectDto dto);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="userId">The ID of the user (must own the project).</param>
    /// <param name="projectId">The ID of the project to update.</param>
    /// <param name="dto">The updated project data.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> UpdateProjectAsync(int userId, int projectId, UpdateProjectDto dto);

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="userId">The ID of the user (must own the project).</param>
    /// <param name="projectId">The ID of the project to delete.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> DeleteProjectAsync(int userId, int projectId);

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>Service response containing the project details.</returns>
    Task<ServiceResponse<ProjectResponseDto>> GetProjectByIdAsync(int projectId);

    /// <summary>
    /// Gets all projects for a company.
    /// </summary>
    /// <param name="userId">The ID of the company user.</param>
    /// <returns>Service response containing the list of projects.</returns>
    Task<ServiceResponse<IEnumerable<ProjectResponseDto>>> GetCompanyProjectsAsync(int userId);

    /// <summary>
    /// Gets filtered and paginated projects.
    /// </summary>
    /// <param name="filter">The filter criteria.</param>
    /// <returns>Service response containing paginated project results.</returns>
    Task<ServiceResponse<PagedResult<ProjectResponseDto>>> GetFilteredProjectsAsync(ProjectFilterDto filter);
}
