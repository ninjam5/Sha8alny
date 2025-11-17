using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.Common;
using Sh8lny.Application.DTOs.Applications;
using Sh8lny.Application.DTOs.Projects;
using Sh8lny.Domain.Entities;
using Sh8lny.IntegrationTests.Fixtures;
using Sh8lny.IntegrationTests.Helpers;
using Sh8lny.Persistence.Contexts;
using DomainApplication = Sh8lny.Domain.Entities.Application;

namespace Sh8lny.IntegrationTests.Controllers;

/// <summary>
/// Integration tests that cover the project curriculum and student progress endpoints
/// </summary>
public class ProjectCurriculumTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProjectCurriculumTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AddModule_AsProjectOwner_ReturnsCreated()
    {
        // Arrange
        var seed = await SeedCompanyProjectAsync();
        AuthenticateAsCompany(seed.CompanyUser);

        var dto = new CreateProjectModuleDto
        {
            Title = "Module 1",
            Description = "Integration test module",
            Duration = "1 week",
            OrderIndex = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/projects/{seed.Project.ProjectID}/modules", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectModuleDto>>();
        payload.Should().NotBeNull();
        payload!.Data.Should().NotBeNull();
        payload.Data!.OrderIndex.Should().Be(1);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var storedModule = await db.ProjectModules.FirstOrDefaultAsync(m => m.ProjectId == seed.Project.ProjectID);
        storedModule.Should().NotBeNull();
        storedModule!.OrderIndex.Should().Be(1);
    }

    [Fact]
    public async Task ReorderModules_UpdatesIndicesCorrectly()
    {
        // Arrange
        var seed = await SeedProjectWithModulesAsync(3);
        AuthenticateAsCompany(seed.CompanyUser);

        var reorderDto = new ReorderProjectModulesDto
        {
            ModuleIds = new List<int> { seed.Modules[2].Id, seed.Modules[0].Id, seed.Modules[1].Id }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/projects/{seed.Project.ProjectID}/modules/reorder", reorderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedModule = await db.ProjectModules.FindAsync(seed.Modules[2].Id);
        updatedModule.Should().NotBeNull();
        updatedModule!.OrderIndex.Should().Be(1);
    }

    [Fact]
    public async Task ToggleModuleProgress_AsStudent_UpdatesPercentage()
    {
        // Arrange
        var seed = await SeedApplicationToggleScenarioAsync();
        AuthenticateAsStudent(seed.ApplicantUser);

        var request = new { IsCompleted = true };
        var targetModuleId = seed.ProjectSeed.Modules.First().Id;

        // Act
        var response = await _client.PostAsJsonAsync($"/api/applications/{seed.Application.ApplicationID}/modules/{targetModuleId}/toggle", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<ApplicationProgressDto>>();
        payload.Should().NotBeNull();
        payload!.Data.Should().NotBeNull();
        payload.Data!.TotalModules.Should().Be(2);
        payload.Data.CompletedModulesCount.Should().Be(1);
        payload.Data.ProgressPercentage.Should().Be(50.0);
        payload.Data.CompletedModuleIds.Should().Contain(targetModuleId);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var progressEntry = await db.ApplicationModuleProgress.FirstOrDefaultAsync(p =>
            p.ApplicationId == seed.Application.ApplicationID && p.ProjectModuleId == targetModuleId);
        progressEntry.Should().NotBeNull();
        progressEntry!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleProgress_AsNonApplicant_ReturnsForbidden()
    {
        // Arrange
        var seed = await SeedApplicationToggleScenarioAsync(includeNonApplicant: true);
        seed.NonApplicant.Should().NotBeNull();
        seed.NonApplicantUser.Should().NotBeNull();
        AuthenticateAsStudent(seed.NonApplicantUser!);

        var request = new { IsCompleted = true };
        var targetModuleId = seed.ProjectSeed.Modules.First().Id;

        // Act
        var response = await _client.PostAsJsonAsync($"/api/applications/{seed.Application.ApplicationID}/modules/{targetModuleId}/toggle", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private void AuthenticateAsCompany(User companyUser)
    {
        var token = JwtTokenHelper.GenerateJwtToken(companyUser.UserID, companyUser.Email, "Company");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private void AuthenticateAsStudent(User studentUser)
    {
        var token = JwtTokenHelper.GenerateJwtToken(studentUser.UserID, studentUser.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<ProjectOwnershipSeed> SeedCompanyProjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = TestDataGenerator.CreateTestCompany(companyUser.UserID);
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        var project = TestDataGenerator.CreateTestProject(company.CompanyID, companyUser.UserID);
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        return new ProjectOwnershipSeed(companyUser, project);
    }

    private async Task<ProjectWithModulesSeed> SeedProjectWithModulesAsync(int moduleCount)
    {
        var ownership = await SeedCompanyProjectAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var modules = new List<ProjectModule>();
        for (var index = 0; index < moduleCount; index++)
        {
            modules.Add(new ProjectModule
            {
                ProjectId = ownership.Project.ProjectID,
                Title = $"Module {index + 1}",
                Description = "Integration test module",
                EstimatedDuration = "1 week",
                OrderIndex = index + 1
            });
        }

        await db.ProjectModules.AddRangeAsync(modules);
        await db.SaveChangesAsync();

        var persistedModules = await db.ProjectModules
            .Where(m => m.ProjectId == ownership.Project.ProjectID)
            .OrderBy(m => m.OrderIndex)
            .AsNoTracking()
            .ToListAsync();

        return new ProjectWithModulesSeed(ownership.CompanyUser, ownership.Project, persistedModules);
    }

    private async Task<ApplicationToggleSeed> SeedApplicationToggleScenarioAsync(bool includeNonApplicant = false)
    {
        var projectSeed = await SeedProjectWithModulesAsync(2);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var applicantUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(applicantUser);
        await db.SaveChangesAsync();

        var applicant = TestDataGenerator.CreateTestStudent(applicantUser.UserID);
        await db.Students.AddAsync(applicant);
        await db.SaveChangesAsync();

        var application = new DomainApplication
        {
            ProjectID = projectSeed.Project.ProjectID,
            StudentID = applicant.StudentID,
            Resume = "https://example.com/resume.pdf",
            Status = ApplicationStatus.Accepted,
            AppliedAt = DateTime.UtcNow,
            CoverLetter = "Ready to contribute"
        };

        await db.Applications.AddAsync(application);
        await db.SaveChangesAsync();

        Student? nonApplicant = null;
        User? nonApplicantUser = null;

        if (includeNonApplicant)
        {
            nonApplicantUser = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(nonApplicantUser);
            await db.SaveChangesAsync();

            nonApplicant = TestDataGenerator.CreateTestStudent(nonApplicantUser.UserID);
            await db.Students.AddAsync(nonApplicant);
            await db.SaveChangesAsync();
        }

        return new ApplicationToggleSeed(projectSeed, application, applicant, applicantUser, nonApplicant, nonApplicantUser);
    }

    private sealed record ProjectOwnershipSeed(User CompanyUser, Project Project);

    private sealed record ProjectWithModulesSeed(User CompanyUser, Project Project, List<ProjectModule> Modules);

    private sealed record ApplicationToggleSeed(
        ProjectWithModulesSeed ProjectSeed,
        DomainApplication Application,
        Student Applicant,
        User ApplicantUser,
        Student? NonApplicant,
        User? NonApplicantUser);
}
