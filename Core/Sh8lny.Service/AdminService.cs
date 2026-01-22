using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Admin;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Service;

/// <summary>
/// Service for admin management operations.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<AdminDashboardStatsDto>> GetDashboardStatsAsync()
    {
        try
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            // User Statistics
            var totalStudents = await _unitOfWork.Students.CountAsync();
            var totalCompanies = await _unitOfWork.Companies.CountAsync();
            var totalUsers = await _unitOfWork.Users.CountAsync();
            var activeUsers = await _unitOfWork.Users.CountAsync(u => u.IsActive);
            var bannedUsers = await _unitOfWork.Users.CountAsync(u => !u.IsActive);
            var newUsersLast30Days = await _unitOfWork.Users.CountAsync(u => u.CreatedAt >= thirtyDaysAgo);

            // Project Statistics
            var totalProjects = await _unitOfWork.Projects.CountAsync();
            var activeProjects = await _unitOfWork.Projects.CountAsync(p => 
                p.Status == ProjectStatus.Active || p.Status == ProjectStatus.Pending);
            var closedProjects = await _unitOfWork.Projects.CountAsync(p => 
                p.Status == ProjectStatus.Closed || p.Status == ProjectStatus.Complete);
            var newProjectsLast30Days = await _unitOfWork.Projects.CountAsync(p => p.CreatedAt >= thirtyDaysAgo);

            // Application Statistics
            var totalApplications = await _unitOfWork.Applications.CountAsync();
            var completedApplications = await _unitOfWork.Applications.CountAsync(a => 
                a.Status == ApplicationStatus.Completed);

            // Financial Statistics
            var transactions = await _unitOfWork.Transactions.GetAllAsync();
            var transactionList = transactions.ToList();
            var totalTransactionVolume = transactionList
                .Where(t => t.Status == TransactionStatus.Completed)
                .Sum(t => t.Amount);
            var totalTransactions = transactionList.Count;

            var stats = new AdminDashboardStatsDto
            {
                TotalStudents = totalStudents,
                TotalCompanies = totalCompanies,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                BannedUsers = bannedUsers,
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                ClosedProjects = closedProjects,
                TotalApplications = totalApplications,
                CompletedApplications = completedApplications,
                TotalTransactionVolume = totalTransactionVolume,
                TotalTransactions = totalTransactions,
                NewUsersLast30Days = newUsersLast30Days,
                NewProjectsLast30Days = newProjectsLast30Days
            };

            return ServiceResponse<AdminDashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return ServiceResponse<AdminDashboardStatsDto>.Failure(
                "An error occurred while retrieving dashboard statistics.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<UserManagementDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = new List<UserManagementDto>();

            foreach (var user in users.OrderByDescending(u => u.CreatedAt))
            {
                var (displayName, profilePicture) = await GetUserDisplayInfoAsync(user);

                userDtos.Add(new UserManagementDto
                {
                    UserId = user.UserID,
                    Email = user.Email,
                    DisplayName = displayName,
                    Role = user.UserType.ToString(),
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified,
                    JoinDate = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    ProfilePicture = profilePicture
                });
            }

            return ServiceResponse<IEnumerable<UserManagementDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<UserManagementDto>>.Failure(
                "An error occurred while retrieving users.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<ProjectManagementDto>>> GetAllProjectsAsync()
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var projectDtos = new List<ProjectManagementDto>();

            foreach (var project in projects.OrderByDescending(p => p.CreatedAt))
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(project.CompanyID);

                projectDtos.Add(new ProjectManagementDto
                {
                    ProjectId = project.ProjectID,
                    ProjectName = project.ProjectName,
                    CompanyName = company?.CompanyName ?? "Unknown Company",
                    Status = project.Status.ToString(),
                    ApplicationCount = project.ApplicationCount,
                    CreatedAt = project.CreatedAt,
                    IsVisible = project.IsVisible
                });
            }

            return ServiceResponse<IEnumerable<ProjectManagementDto>>.Success(projectDtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<ProjectManagementDto>>.Failure(
                "An error occurred while retrieving projects.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> ToggleUserBanAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<bool>.Failure("User not found.");
            }

            // Prevent banning other admins
            if (user.UserType == UserType.Admin)
            {
                return ServiceResponse<bool>.Failure("Cannot ban an admin user.");
            }

            // Toggle the IsActive status
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            var action = user.IsActive ? "unbanned" : "banned";
            return ServiceResponse<bool>.Success(user.IsActive, $"User has been {action} successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while updating user status.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> DeleteProjectForceAsync(int projectId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (project is null)
            {
                return ServiceResponse<bool>.Failure("Project not found.");
            }

            // Admin bypass - no ownership check required
            // Delete related entities first (to avoid FK constraints)

            // Delete project modules
            var modules = await _unitOfWork.ProjectModules.FindAsync(m => m.ProjectId == projectId);
            foreach (var module in modules)
            {
                // Delete module progress records
                var progressRecords = await _unitOfWork.ApplicationModuleProgress
                    .FindAsync(p => p.ProjectModuleId == module.Id);
                _unitOfWork.ApplicationModuleProgress.RemoveRange(progressRecords);
                
                _unitOfWork.ProjectModules.Remove(module);
            }

            // Delete applications
            var applications = await _unitOfWork.Applications.FindAsync(a => a.ProjectID == projectId);
            _unitOfWork.Applications.RemoveRange(applications);

            // Delete required skills
            var requiredSkills = await _unitOfWork.ProjectRequiredSkills
                .FindAsync(s => s.ProjectID == projectId);
            _unitOfWork.ProjectRequiredSkills.RemoveRange(requiredSkills);

            // Finally delete the project
            _unitOfWork.Projects.Remove(project);

            await _unitOfWork.SaveAsync();

            return ServiceResponse<bool>.Success(true, $"Project '{project.ProjectName}' has been deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while deleting the project.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<UserManagementDto>> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<UserManagementDto>.Failure("User not found.");
            }

            var (displayName, profilePicture) = await GetUserDisplayInfoAsync(user);

            var dto = new UserManagementDto
            {
                UserId = user.UserID,
                Email = user.Email,
                DisplayName = displayName,
                Role = user.UserType.ToString(),
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                JoinDate = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                ProfilePicture = profilePicture
            };

            return ServiceResponse<UserManagementDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ServiceResponse<UserManagementDto>.Failure(
                "An error occurred while retrieving user.",
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Gets display name and profile picture for a user based on their type.
    /// </summary>
    private async Task<(string DisplayName, string? ProfilePicture)> GetUserDisplayInfoAsync(User user)
    {
        switch (user.UserType)
        {
            case UserType.Student:
                var student = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == user.UserID);
                if (student is not null)
                {
                    return (student.FullName, student.ProfilePicture);
                }
                break;

            case UserType.Company:
                var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == user.UserID);
                if (company is not null)
                {
                    return (company.CompanyName, company.CompanyLogo);
                }
                break;

            case UserType.Admin:
                return ($"{user.FirstName ?? "Admin"} {user.LastName ?? ""}".Trim(), null);
        }

        // Fallback
        return (user.Email, null);
    }
}
