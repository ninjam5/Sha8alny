using System.Collections.Generic;
using System.Linq.Expressions;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Projects;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Projects;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ProjectService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<ProjectDetailDto>> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var includes = new Expression<Func<Project, object>>[]
            {
                p => p.Modules
            };

            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, includes, cancellationToken);
            if (project == null)
                return ApiResponse<ProjectDetailDto>.FailureResponse("Project not found");

            var company = await _unitOfWork.Companies.GetByIdAsync(project.CompanyID, cancellationToken);
            var requiredSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(projectId, cancellationToken);
            var orderedModules = project.Modules
                .OrderBy(m => m.OrderIndex)
                .ToList();

            var dto = new ProjectDetailDto
            {
                ProjectID = project.ProjectID,
                CompanyID = project.CompanyID,
                CompanyName = company?.CompanyName ?? string.Empty,
                CompanyLogo = company?.CompanyLogo,
                ProjectName = project.ProjectName,
                ProjectCode = project.ProjectCode,
                Description = project.Description,
                ProjectType = project.ProjectType?.ToString(),
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Deadline = project.Deadline,
                Duration = project.Duration,
                RequiredSkills = requiredSkills.Select(rs => rs.Skill.SkillName).ToList(),
                Modules = orderedModules.Select(MapModule).ToList(),
                MinAcademicYear = project.MinAcademicYear,
                MaxApplicants = project.MaxApplicants,
                CompensationType = null, // Not in entity
                CompensationAmount = null, // Not in entity
                Currency = null, // Not in entity
                Status = project.Status.ToString(),
                IsVisible = project.IsVisible,
                ViewCount = project.ViewCount,
                ApplicationCount = project.ApplicationCount,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };

            return ApiResponse<ProjectDetailDto>.SuccessResponse(dto, "Project retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProjectDetailDto>.FailureResponse($"Error retrieving project: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<ProjectListDto>>> GetProjectsAsync(
        ProjectFilterDto filter, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);

            // Apply filters
            if (filter.CompanyID.HasValue)
                projects = projects.Where(p => p.CompanyID == filter.CompanyID.Value);

            if (!string.IsNullOrWhiteSpace(filter.ProjectType))
            {
                if (Enum.TryParse<ProjectType>(filter.ProjectType, out var projectType))
                    projects = projects.Where(p => p.ProjectType == projectType);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<ProjectStatus>(filter.Status, out var status))
                    projects = projects.Where(p => p.Status == status);
            }

            if (filter.DeadlineAfter.HasValue)
                projects = projects.Where(p => p.Deadline >= filter.DeadlineAfter.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchLower = filter.SearchTerm.ToLower();
                projects = projects.Where(p =>
                    p.ProjectName.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower));
            }

            if (filter.SkillID.HasValue)
            {
                var projectIdsWithSkill = (await _unitOfWork.ProjectRequiredSkills.GetAllAsync(cancellationToken))
                    .Where(prs => prs.SkillID == filter.SkillID.Value)
                    .Select(prs => prs.ProjectID)
                    .Distinct();
                projects = projects.Where(p => projectIdsWithSkill.Contains(p.ProjectID));
            }

            var totalCount = projects.Count();

            // Pagination
            var paginatedProjects = projects
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var projectDtos = new List<ProjectListDto>();
            foreach (var p in paginatedProjects)
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(p.CompanyID, cancellationToken);
                var requiredSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(p.ProjectID, cancellationToken);
                
                projectDtos.Add(new ProjectListDto
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    CompanyName = company?.CompanyName ?? string.Empty,
                    CompanyLogo = company?.CompanyLogo,
                    ProjectType = p.ProjectType?.ToString(),
                    Deadline = p.Deadline,
                    RequiredSkills = requiredSkills.Select(rs => rs.Skill.SkillName).ToList(),
                    CompensationType = null,
                    CompensationAmount = null,
                    ApplicationCount = p.ApplicationCount,
                    MaxApplicants = p.MaxApplicants,
                    CreatedAt = p.CreatedAt
                });
            }

            var result = new PagedResult<ProjectListDto>
            {
                Items = projectDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ApiResponse<PagedResult<ProjectListDto>>.SuccessResponse(result, "Projects retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<ProjectListDto>>.FailureResponse($"Error retrieving projects: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProjectListDto>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetActiveProjectsAsync(cancellationToken);

            var projectDtos = new List<ProjectListDto>();
            foreach (var p in projects)
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(p.CompanyID, cancellationToken);
                var requiredSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(p.ProjectID, cancellationToken);
                
                projectDtos.Add(new ProjectListDto
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    CompanyName = company?.CompanyName ?? string.Empty,
                    CompanyLogo = company?.CompanyLogo,
                    ProjectType = p.ProjectType?.ToString(),
                    Deadline = p.Deadline,
                    RequiredSkills = requiredSkills.Select(rs => rs.Skill.SkillName).ToList(),
                    CompensationType = null,
                    CompensationAmount = null,
                    ApplicationCount = p.ApplicationCount,
                    MaxApplicants = p.MaxApplicants,
                    CreatedAt = p.CreatedAt
                });
            }

            return ApiResponse<List<ProjectListDto>>.SuccessResponse(projectDtos, "Active projects retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProjectListDto>>.FailureResponse($"Error retrieving active projects: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProjectListDto>>> GetProjectsByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetByCompanyIdAsync(companyId, cancellationToken);
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);

            var projectDtos = new List<ProjectListDto>();
            foreach (var p in projects)
            {
                var requiredSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(p.ProjectID, cancellationToken);
                
                projectDtos.Add(new ProjectListDto
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    CompanyName = company?.CompanyName ?? string.Empty,
                    CompanyLogo = company?.CompanyLogo,
                    ProjectType = p.ProjectType?.ToString(),
                    Deadline = p.Deadline,
                    RequiredSkills = requiredSkills.Select(rs => rs.Skill.SkillName).ToList(),
                    CompensationType = null,
                    CompensationAmount = null,
                    ApplicationCount = p.ApplicationCount,
                    MaxApplicants = p.MaxApplicants,
                    CreatedAt = p.CreatedAt
                });
            }

            return ApiResponse<List<ProjectListDto>>.SuccessResponse(projectDtos, "Company projects retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProjectListDto>>.FailureResponse($"Error retrieving company projects: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProjectListDto>>> SearchProjectsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var projects = await _unitOfWork.Projects.SearchProjectsAsync(searchTerm, cancellationToken);

            var projectDtos = new List<ProjectListDto>();
            foreach (var p in projects)
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(p.CompanyID, cancellationToken);
                var requiredSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(p.ProjectID, cancellationToken);
                
                projectDtos.Add(new ProjectListDto
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    CompanyName = company?.CompanyName ?? string.Empty,
                    CompanyLogo = company?.CompanyLogo,
                    ProjectType = p.ProjectType?.ToString(),
                    Deadline = p.Deadline,
                    RequiredSkills = requiredSkills.Select(rs => rs.Skill.SkillName).ToList(),
                    CompensationType = null,
                    CompensationAmount = null,
                    ApplicationCount = p.ApplicationCount,
                    MaxApplicants = p.MaxApplicants,
                    CreatedAt = p.CreatedAt
                });
            }

            return ApiResponse<List<ProjectListDto>>.SuccessResponse(projectDtos, "Projects found successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProjectListDto>>.FailureResponse($"Error searching projects: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProjectDetailDto>> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(dto.CompanyID, cancellationToken);
            if (company == null)
                return ApiResponse<ProjectDetailDto>.FailureResponse("Company not found");

            var project = new Project
            {
                CompanyID = dto.CompanyID,
                ProjectName = dto.ProjectName,
                ProjectCode = dto.ProjectCode,
                Description = dto.Description,
                ProjectType = !string.IsNullOrWhiteSpace(dto.ProjectType) && Enum.TryParse<ProjectType>(dto.ProjectType, out var pType)
                    ? pType
                    : null,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Deadline = dto.Deadline,
                Duration = dto.Duration,
                MinAcademicYear = dto.MinAcademicYear,
                MaxApplicants = dto.MaxApplicants,
                Status = ProjectStatus.Draft,
                IsVisible = false,
                CreatedBy = company.UserID,
                CreatedByName = company.CompanyName,
                ViewCount = 0,
                ApplicationCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Projects.AddAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Add required skills
            if (dto.RequiredSkillIDs != null && dto.RequiredSkillIDs.Any())
            {
                foreach (var skillId in dto.RequiredSkillIDs)
                {
                    var skill = await _unitOfWork.Skills.GetByIdAsync(skillId, cancellationToken);
                    if (skill != null)
                    {
                        var projectRequiredSkill = new ProjectRequiredSkill
                        {
                            ProjectID = project.ProjectID,
                            SkillID = skillId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.ProjectRequiredSkills.AddAsync(projectRequiredSkill, cancellationToken);
                    }
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return await GetProjectByIdAsync(project.ProjectID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProjectDetailDto>.FailureResponse($"Error creating project: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProjectDetailDto>> UpdateProjectAsync(int projectId, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
                return ApiResponse<ProjectDetailDto>.FailureResponse("Project not found");

            if (!string.IsNullOrWhiteSpace(dto.ProjectName))
                project.ProjectName = dto.ProjectName;
            if (dto.Description != null)
                project.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.ProjectType) && Enum.TryParse<ProjectType>(dto.ProjectType, out var pType))
                project.ProjectType = pType;
            if (dto.StartDate.HasValue)
                project.StartDate = dto.StartDate;
            if (dto.EndDate.HasValue)
                project.EndDate = dto.EndDate;
            if (dto.Deadline.HasValue)
                project.Deadline = dto.Deadline.Value;
            if (dto.Duration != null)
                project.Duration = dto.Duration;
            if (dto.MinAcademicYear != null)
                project.MinAcademicYear = dto.MinAcademicYear;
            if (dto.MaxApplicants.HasValue)
                project.MaxApplicants = dto.MaxApplicants;
            if (dto.IsVisible.HasValue)
                project.IsVisible = dto.IsVisible.Value;
            
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);

            // Update required skills if provided
            if (dto.RequiredSkillIDs != null)
            {
                var existingSkills = await _unitOfWork.ProjectRequiredSkills.GetByProjectIdAsync(projectId, cancellationToken);
                foreach (var existingSkill in existingSkills)
                {
                    await _unitOfWork.ProjectRequiredSkills.DeleteAsync(existingSkill, cancellationToken);
                }

                foreach (var skillId in dto.RequiredSkillIDs)
                {
                    var skill = await _unitOfWork.Skills.GetByIdAsync(skillId, cancellationToken);
                    if (skill != null)
                    {
                        var projectRequiredSkill = new ProjectRequiredSkill
                        {
                            ProjectID = projectId,
                            SkillID = skillId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.ProjectRequiredSkills.AddAsync(projectRequiredSkill, cancellationToken);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetProjectByIdAsync(projectId, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProjectDetailDto>.FailureResponse($"Error updating project: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
                return ApiResponse<bool>.FailureResponse("Project not found");

            await _unitOfWork.Projects.DeleteAsync(projectId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Project deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error deleting project: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateProjectStatusAsync(int projectId, ProjectStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
                return ApiResponse<bool>.FailureResponse("Project not found");

            project.Status = status;
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, $"Project status updated to {status}");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error updating project status: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> IncrementViewCountAsync(int projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Projects.IncrementViewCountAsync(projectId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "View count incremented");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error incrementing view count: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProjectModuleDto>> AddModuleAsync(int projectId, CreateProjectModuleDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
                return ApiResponse<ProjectModuleDto>.FailureResponse("Project not found");

            var ownershipCheck = await EnsureProjectOwnershipAsync(project, cancellationToken);
            if (!ownershipCheck.Success)
                return ApiResponse<ProjectModuleDto>.FailureResponse(ownershipCheck.Message ?? "Unauthorized");

            var existingModules = (await _unitOfWork.ProjectModules.GetByProjectIdAsync(projectId, cancellationToken)).ToList();
            var insertOrder = CalculateInsertOrder(dto.OrderIndex, existingModules.Count);

            if (insertOrder <= existingModules.Count)
            {
                foreach (var module in existingModules.Where(m => m.OrderIndex >= insertOrder))
                {
                    module.OrderIndex++;
                }

                await _unitOfWork.ProjectModules.UpdateRangeAsync(existingModules, cancellationToken);
            }

            var newModule = new ProjectModule
            {
                ProjectId = projectId,
                Title = dto.Title,
                Description = dto.Description,
                EstimatedDuration = dto.Duration,
                OrderIndex = insertOrder
            };

            await _unitOfWork.ProjectModules.AddAsync(newModule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProjectModuleDto>.SuccessResponse(MapModule(newModule), "Project module added successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProjectModuleDto>.FailureResponse($"Error adding project module: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteModuleAsync(int moduleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _unitOfWork.ProjectModules.GetByIdAsync(moduleId, cancellationToken);
            if (module == null)
                return ApiResponse<bool>.FailureResponse("Module not found");

            var project = await _unitOfWork.Projects.GetByIdAsync(module.ProjectId, cancellationToken);
            if (project == null)
                return ApiResponse<bool>.FailureResponse("Project not found");

            var ownershipCheck = await EnsureProjectOwnershipAsync(project, cancellationToken);
            if (!ownershipCheck.Success)
                return ApiResponse<bool>.FailureResponse(ownershipCheck.Message ?? "Unauthorized");

            await _unitOfWork.ProjectModules.DeleteAsync(module, cancellationToken);

            var remainingModules = (await _unitOfWork.ProjectModules.GetByProjectIdAsync(module.ProjectId, cancellationToken)).ToList();
            ReindexModules(remainingModules);

            if (remainingModules.Any())
            {
                await _unitOfWork.ProjectModules.UpdateRangeAsync(remainingModules, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.SuccessResponse(true, "Module deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error deleting project module: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> ReorderModulesAsync(int projectId, List<int> moduleIdsInNewOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            if (moduleIdsInNewOrder == null || moduleIdsInNewOrder.Count == 0)
                return ApiResponse<bool>.FailureResponse("Module order payload is required");

            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
            if (project == null)
                return ApiResponse<bool>.FailureResponse("Project not found");

            var ownershipCheck = await EnsureProjectOwnershipAsync(project, cancellationToken);
            if (!ownershipCheck.Success)
                return ApiResponse<bool>.FailureResponse(ownershipCheck.Message ?? "Unauthorized");

            var modules = (await _unitOfWork.ProjectModules.GetByProjectIdAsync(projectId, cancellationToken)).ToList();
            if (!modules.Any())
                return ApiResponse<bool>.FailureResponse("No modules found for project");

            if (moduleIdsInNewOrder.Distinct().Count() != modules.Count)
                return ApiResponse<bool>.FailureResponse("Module order list does not match existing modules");

            var moduleLookup = modules.ToDictionary(m => m.Id);
            if (moduleIdsInNewOrder.Any(id => !moduleLookup.ContainsKey(id)))
                return ApiResponse<bool>.FailureResponse("Module order contains invalid identifiers");

            var order = 1;
            foreach (var moduleId in moduleIdsInNewOrder)
            {
                moduleLookup[moduleId].OrderIndex = order++;
            }

            await _unitOfWork.ProjectModules.UpdateRangeAsync(moduleLookup.Values, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Modules reordered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error reordering modules: {ex.Message}");
        }
    }

    private async Task<ApiResponse<bool>> EnsureProjectOwnershipAsync(Project project, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
            return ApiResponse<bool>.FailureResponse("User authentication required");

        var currentUserId = _currentUserService.UserId.Value;
        if (project.CreatedBy == currentUserId)
            return ApiResponse<bool>.SuccessResponse(true);

        var company = await _unitOfWork.Companies.GetByIdAsync(project.CompanyID, cancellationToken);
        if (company != null && company.UserID == currentUserId)
            return ApiResponse<bool>.SuccessResponse(true);

        return ApiResponse<bool>.FailureResponse("You are not authorized to modify this project");
    }

    private static ProjectModuleDto MapModule(ProjectModule module) => new()
    {
        ProjectModuleId = module.Id,
        ProjectId = module.ProjectId,
        Title = module.Title,
        Description = module.Description,
        Duration = module.EstimatedDuration,
        OrderIndex = module.OrderIndex
    };

    private static int CalculateInsertOrder(int? requestedOrder, int existingCount)
    {
        if (!requestedOrder.HasValue || requestedOrder.Value <= 0)
            return existingCount + 1;

        return requestedOrder.Value > existingCount + 1
            ? existingCount + 1
            : requestedOrder.Value;
    }

    private static void ReindexModules(IList<ProjectModule> modules)
    {
        var order = 1;
        foreach (var module in modules.OrderBy(m => m.OrderIndex))
        {
            module.OrderIndex = order++;
        }
    }
}
