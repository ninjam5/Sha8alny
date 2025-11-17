using System;
using System.Collections.Generic;
using System.Linq;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Applications;
using Sh8lny.Application.DTOs.ActivityLogs;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Interfaces;
using DomainApplication = Sh8lny.Domain.Entities.Application;

namespace Sh8lny.Application.UseCases.Applications;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityLogService _activityLogService;
    private readonly ICurrentUserService _currentUserService;

    public ApplicationService(IUnitOfWork unitOfWork, IActivityLogService activityLogService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _activityLogService = activityLogService;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<ApplicationDetailDto>> GetApplicationByIdAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var application = await _unitOfWork.Applications.GetWithProgressAsync(applicationId, cancellationToken);
            if (application == null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Application not found");

            var project = application.Project ?? await _unitOfWork.Projects.GetByIdAsync(application.ProjectID, cancellationToken);
            var student = await _unitOfWork.Students.GetByIdAsync(application.StudentID, cancellationToken);
            var company = project != null ? await _unitOfWork.Companies.GetByIdAsync(project.CompanyID, cancellationToken) : null;
            var studentUser = student != null ? await _unitOfWork.Users.GetByIdAsync(student.UserID, cancellationToken) : null;

            User? reviewer = null;
            if (application.ReviewedBy.HasValue)
                reviewer = await _unitOfWork.Users.GetByIdAsync(application.ReviewedBy.Value, cancellationToken);

            var progress = BuildProgressSnapshot(application);

            var dto = new ApplicationDetailDto
            {
                ApplicationID = application.ApplicationID,
                ProjectID = application.ProjectID,
                ProjectName = project?.ProjectName ?? string.Empty,
                CompanyName = company?.CompanyName ?? string.Empty,
                StudentID = application.StudentID,
                StudentName = student?.FullName ?? string.Empty,
                StudentEmail = studentUser?.Email,
                StudentPhone = student?.Phone,
                CoverLetter = application.CoverLetter,
                Resume = application.Resume,
                PortfolioURL = application.PortfolioURL,
                ProposalDocument = application.ProposalDocument,
                Status = application.Status.ToString(),
                ReviewedBy = application.ReviewedBy,
                ReviewerName = reviewer?.Email,
                ReviewedAt = application.ReviewedAt,
                ReviewNotes = application.ReviewNotes,
                AppliedAt = application.AppliedAt,
                TotalModules = progress.TotalModules,
                CompletedModules = progress.CompletedModulesCount,
                ProgressPercentage = progress.ProgressPercentage,
                CompletedModuleIds = progress.CompletedModuleIds
            };

            return ApiResponse<ApplicationDetailDto>.SuccessResponse(dto, "Application retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ApplicationDetailDto>.FailureResponse($"Error retrieving application: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<ApplicationListDto>>> GetApplicationsAsync(
        ApplicationFilterDto filter, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var applications = await _unitOfWork.Applications.GetAllAsync(cancellationToken);

            // Apply filters
            if (filter.ProjectID.HasValue)
                applications = applications.Where(a => a.ProjectID == filter.ProjectID.Value);

            if (filter.StudentID.HasValue)
                applications = applications.Where(a => a.StudentID == filter.StudentID.Value);

            if (filter.Status.HasValue)
                applications = applications.Where(a => a.Status == filter.Status.Value);

            if (filter.AppliedAfter.HasValue)
                applications = applications.Where(a => a.AppliedAt >= filter.AppliedAfter.Value);

            if (filter.AppliedBefore.HasValue)
                applications = applications.Where(a => a.AppliedAt <= filter.AppliedBefore.Value);

            if (filter.CompanyID.HasValue)
            {
                var companyProjects = await _unitOfWork.Projects.GetByCompanyIdAsync(filter.CompanyID.Value, cancellationToken);
                var projectIds = companyProjects.Select(p => p.ProjectID).ToList();
                applications = applications.Where(a => projectIds.Contains(a.ProjectID));
            }

            var totalCount = applications.Count();

            // Pagination
            var paginatedApplications = applications
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var applicationDtos = new List<ApplicationListDto>();
            foreach (var app in paginatedApplications)
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(app.ProjectID, cancellationToken);
                var student = await _unitOfWork.Students.GetByIdAsync(app.StudentID, cancellationToken);

                applicationDtos.Add(new ApplicationListDto
                {
                    ApplicationID = app.ApplicationID,
                    ProjectID = app.ProjectID,
                    ProjectName = project?.ProjectName ?? string.Empty,
                    StudentID = app.StudentID,
                    StudentName = student?.FullName ?? string.Empty,
                    StudentProfilePicture = student?.ProfilePicture,
                    Status = app.Status.ToString(),
                    AppliedAt = app.AppliedAt
                });
            }

            var result = new PagedResult<ApplicationListDto>
            {
                Items = applicationDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ApiResponse<PagedResult<ApplicationListDto>>.SuccessResponse(result, "Applications retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<ApplicationListDto>>.FailureResponse($"Error retrieving applications: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ApplicationListDto>>> GetApplicationsByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var applications = await _unitOfWork.Applications.GetByProjectIdAsync(projectId, cancellationToken);
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);

            var applicationDtos = new List<ApplicationListDto>();
            foreach (var app in applications)
            {
                var student = await _unitOfWork.Students.GetByIdAsync(app.StudentID, cancellationToken);

                applicationDtos.Add(new ApplicationListDto
                {
                    ApplicationID = app.ApplicationID,
                    ProjectID = app.ProjectID,
                    ProjectName = project?.ProjectName ?? string.Empty,
                    StudentID = app.StudentID,
                    StudentName = student?.FullName ?? string.Empty,
                    StudentProfilePicture = student?.ProfilePicture,
                    Status = app.Status.ToString(),
                    AppliedAt = app.AppliedAt
                });
            }

            return ApiResponse<List<ApplicationListDto>>.SuccessResponse(applicationDtos, "Project applications retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ApplicationListDto>>.FailureResponse($"Error retrieving project applications: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ApplicationListDto>>> GetApplicationsByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var applications = await _unitOfWork.Applications.GetByStudentIdAsync(studentId, cancellationToken);
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);

            var applicationDtos = new List<ApplicationListDto>();
            foreach (var app in applications)
            {
                var project = await _unitOfWork.Projects.GetByIdAsync(app.ProjectID, cancellationToken);

                applicationDtos.Add(new ApplicationListDto
                {
                    ApplicationID = app.ApplicationID,
                    ProjectID = app.ProjectID,
                    ProjectName = project?.ProjectName ?? string.Empty,
                    StudentID = app.StudentID,
                    StudentName = student?.FullName ?? string.Empty,
                    StudentProfilePicture = student?.ProfilePicture,
                    Status = app.Status.ToString(),
                    AppliedAt = app.AppliedAt
                });
            }

            return ApiResponse<List<ApplicationListDto>>.SuccessResponse(applicationDtos, "Student applications retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ApplicationListDto>>.FailureResponse($"Error retrieving student applications: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ApplicationDetailDto>> SubmitApplicationAsync(SubmitApplicationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(dto.ProjectID, cancellationToken);
            if (project == null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Project not found");

            var student = await _unitOfWork.Students.GetByIdAsync(dto.StudentID, cancellationToken);
            if (student == null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Student not found");

            // Check if already applied
            var existingApplication = await _unitOfWork.Applications.GetByProjectAndStudentAsync(dto.ProjectID, dto.StudentID, cancellationToken);
            if (existingApplication != null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Student has already applied to this project");

            // Check if project accepts applications
            if (project.Status != ProjectStatus.Active)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Project is not accepting applications");

            // Check max applicants
            if (project.MaxApplicants.HasValue && project.ApplicationCount >= project.MaxApplicants.Value)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Project has reached maximum applicants");

            var application = new Domain.Entities.Application
            {
                ProjectID = dto.ProjectID,
                StudentID = dto.StudentID,
                CoverLetter = dto.CoverLetter,
                Resume = dto.Resume,
                PortfolioURL = dto.PortfolioURL,
                ProposalDocument = dto.ProposalDocument,
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            await _unitOfWork.Applications.AddAsync(application, cancellationToken);

            // Increment project application count
            project.ApplicationCount++;
            await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Log student's action
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = student.UserID,
                ActivityType = "ApplicationSubmitted",
                Description = $"Student {student.FirstName} {student.LastName} applied to project {project.ProjectName}",
                RelatedEntityType = "Application",
                RelatedEntityID = application.ApplicationID
            });

            return await GetApplicationByIdAsync(application.ApplicationID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<ApplicationDetailDto>.FailureResponse($"Error submitting application: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ApplicationDetailDto>> ReviewApplicationAsync(ReviewApplicationDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(dto.ApplicationID, cancellationToken);
            if (application == null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Application not found");

            var reviewer = await _unitOfWork.Users.GetByIdAsync(dto.ReviewedBy, cancellationToken);
            if (reviewer == null)
                return ApiResponse<ApplicationDetailDto>.FailureResponse("Reviewer not found");

            application.Status = dto.Status;
            application.ReviewedBy = dto.ReviewedBy;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewNotes = dto.ReviewNotes;

            await _unitOfWork.Applications.UpdateAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Log company's action
            var actionType = dto.Status == ApplicationStatus.Accepted ? "ApplicationAccepted" : "ApplicationRejected";
            await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
            {
                UserID = dto.ReviewedBy,
                ActivityType = actionType,
                Description = $"Company reviewed application {application.ApplicationID} - Status: {dto.Status}",
                RelatedEntityType = "Application",
                RelatedEntityID = application.ApplicationID
            });

            return await GetApplicationByIdAsync(application.ApplicationID, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<ApplicationDetailDto>.FailureResponse($"Error reviewing application: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> WithdrawApplicationAsync(int applicationId, int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var application = await _unitOfWork.Applications.GetByIdAsync(applicationId, cancellationToken);
            if (application == null)
                return ApiResponse<bool>.FailureResponse("Application not found");

            if (application.StudentID != studentId)
                return ApiResponse<bool>.FailureResponse("Unauthorized to withdraw this application");

            if (application.Status == ApplicationStatus.Accepted || application.Status == ApplicationStatus.Rejected)
                return ApiResponse<bool>.FailureResponse("Cannot withdraw application after it has been reviewed");

            application.Status = ApplicationStatus.Withdrawn;
            await _unitOfWork.Applications.UpdateAsync(application, cancellationToken);

            // Decrement project application count
            var project = await _unitOfWork.Projects.GetByIdAsync(application.ProjectID, cancellationToken);
            if (project != null && project.ApplicationCount > 0)
            {
                project.ApplicationCount--;
                await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Log student's action
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student != null)
            {
                await _activityLogService.CreateActivityLogAsync(new CreateActivityLogDto
                {
                    UserID = student.UserID,
                    ActivityType = "ApplicationWithdrawn",
                    Description = $"Student {student.FirstName} {student.LastName} withdrew application {applicationId}",
                    RelatedEntityType = "Application",
                    RelatedEntityID = applicationId
                });
            }

            return ApiResponse<bool>.SuccessResponse(true, "Application withdrawn successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error withdrawing application: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> HasStudentAppliedAsync(int projectId, int studentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var application = await _unitOfWork.Applications.GetByProjectAndStudentAsync(projectId, studentId, cancellationToken);
            var hasApplied = application != null;

            return ApiResponse<bool>.SuccessResponse(hasApplied, hasApplied ? "Student has applied" : "Student has not applied");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResponse($"Error checking application: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ApplicationProgressDto>> ToggleModuleCompletionAsync(int applicationId, int moduleId, bool isCompleted, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("User authentication required");

            var application = await _unitOfWork.Applications.GetWithProgressAsync(applicationId, cancellationToken);
            if (application == null)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("Application not found");

            var student = await _unitOfWork.Students.GetByIdAsync(application.StudentID, cancellationToken);
            if (student == null)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("Student not found");

            var studentUser = await _unitOfWork.Users.GetByIdAsync(student.UserID, cancellationToken);
            if (studentUser == null || studentUser.UserID != _currentUserService.UserId.Value)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("You are not authorized to update this application");

            var project = application.Project ?? await _unitOfWork.Projects.GetByIdAsync(application.ProjectID, cancellationToken);
            if (project == null)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("Project not found");

            var module = project.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("Module does not belong to this project");

            var existingProgress = application.ModuleProgress?.FirstOrDefault(mp => mp.ProjectModuleId == moduleId);

            if (isCompleted)
            {
                if (existingProgress == null)
                {
                    var entry = new ApplicationModuleProgress
                    {
                        ApplicationId = applicationId,
                        ProjectModuleId = moduleId,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.ApplicationModuleProgresses.AddAsync(entry, cancellationToken);
                }
                else if (!existingProgress.IsCompleted)
                {
                    existingProgress.IsCompleted = true;
                    existingProgress.CompletedAt = DateTime.UtcNow;
                    await _unitOfWork.ApplicationModuleProgresses.UpdateAsync(existingProgress, cancellationToken);
                }
            }
            else if (existingProgress != null)
            {
                await _unitOfWork.ApplicationModuleProgresses.DeleteAsync(existingProgress, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedApplication = await _unitOfWork.Applications.GetWithProgressAsync(applicationId, cancellationToken);
            if (updatedApplication == null)
                return ApiResponse<ApplicationProgressDto>.FailureResponse("Application not found after update");

            var progress = BuildProgressSnapshot(updatedApplication);

            return ApiResponse<ApplicationProgressDto>.SuccessResponse(progress, "Module progress updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ApplicationProgressDto>.FailureResponse($"Error updating module progress: {ex.Message}");
        }
    }

    private static ApplicationProgressDto BuildProgressSnapshot(DomainApplication application)
    {
        var projectModuleIds = application.Project?.Modules?
            .Select(m => m.Id)
            .ToHashSet() ?? new HashSet<int>();

        var totalModules = projectModuleIds.Count;

        var completedModuleIds = application.ModuleProgress?
            .Where(mp => mp.IsCompleted && projectModuleIds.Contains(mp.ProjectModuleId))
            .Select(mp => mp.ProjectModuleId)
            .Distinct()
            .ToList() ?? new List<int>();

        var completedModules = completedModuleIds.Count;
        var progressPercentage = totalModules == 0
            ? 0
            : Math.Round((double)completedModules / totalModules * 100, 2);

        return new ApplicationProgressDto
        {
            ApplicationId = application.ApplicationID,
            ProjectId = application.Project?.ProjectID ?? application.ProjectID,
            TotalModules = totalModules,
            CompletedModulesCount = completedModules,
            ProgressPercentage = progressPercentage,
            CompletedModuleIds = completedModuleIds
        };
    }
}
