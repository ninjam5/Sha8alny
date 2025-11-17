using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.DashboardMetrics;
using Sh8lny.Domain.Entities;
using Sh8lny.IntegrationTests.Fixtures;
using Sh8lny.IntegrationTests.Helpers;
using Sh8lny.Persistence.Contexts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Sh8lny.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for DashboardMetricsController
/// </summary>
public class DashboardMetricsControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DashboardMetricsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPlatformOverview_AsAdmin_ReturnsCorrectCounts()
    {
        // Arrange - Clear database first to ensure clean state
        await ClearDatabaseAsync();
        
        // Arrange - Seed realistic dataset with exact counts
        var adminUser = await SeedAdminUserAsync();
        
        // Seed 5 Student Users with Student profiles
        var students = await SeedStudentsAsync(5);
        
        // Seed 3 Company Users with Company profiles
        var companies = await SeedCompaniesAsync(3);
        
        // Seed 10 Projects (6 Active, 2 Complete, 2 Draft)
        var projects = await SeedProjectsAsync(companies, 10);
        
        // Seed 25 Applications (10 Pending, 8 Accepted, 7 Rejected)
        await SeedApplicationsAsync(students, projects, 25);
        
        // Seed 8 Company Reviews (5 Approved, 3 Pending)
        await SeedCompanyReviewsAsync(companies, students, approvedCount: 5, pendingCount: 3);
        
        // Seed 6 Student Reviews (4 Approved, 2 Pending)
        await SeedStudentReviewsAsync(students, companies, approvedCount: 4, pendingCount: 2);
        
        // Seed 4 Certificates
        await SeedCertificatesAsync(companies, students, 4);

        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dashboardmetrics/platform-overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PlatformOverviewDto>();
        result.Should().NotBeNull();
        
        // Assert exact counts as specified in requirements
        result!.TotalStudents.Should().Be(5, "we seeded 5 students");
        result.TotalCompanies.Should().Be(3, "we seeded 3 companies");
        result.TotalProjects.Should().Be(10, "we seeded 10 projects total");
        result.ActiveProjects.Should().Be(6, "we seeded 6 active projects");
        result.CompletedProjects.Should().Be(2, "we seeded 2 completed projects");
        result.TotalApplications.Should().Be(25, "we seeded 25 applications");
        result.PendingApplications.Should().Be(10, "we seeded 10 pending applications");
        result.AcceptedApplications.Should().Be(8, "we seeded 8 accepted applications");
        
        // Reviews should only count approved ones
        result.TotalReviews.Should().Be(9, "5 approved company reviews + 4 approved student reviews");
        
        // Average ratings should be calculated from approved reviews
        result.AverageCompanyRating.Should().BeGreaterThan(0);
        result.AverageStudentRating.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetPlatformOverview_AsStudent_ReturnsForbidden()
    {
        // Arrange - Clear database first to ensure clean state
        await ClearDatabaseAsync();
        
        // Arrange - Seed a Student user
        var studentUser = await SeedStudentUserAsync();
        
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dashboardmetrics/platform-overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden, "only admins can access platform overview");
    }

    [Fact]
    public async Task GetPlatformOverview_ReturnsCorrectStatusCounts()
    {
        // Arrange - Clear database first to ensure clean state
        await ClearDatabaseAsync();
        
        // Arrange - Seed an Admin user
        var adminUser = await SeedAdminUserAsync();
        
        // Seed companies for projects
        var companies = await SeedCompaniesAsync(2);
        
        // Seed 3 Projects (Status = ProjectStatus.Active)
        var activeProjects = await SeedProjectsWithStatusAsync(companies[0], ProjectStatus.Active, 3);
        
        // Seed 2 Projects (Status = ProjectStatus.Complete)
        var completedProjects = await SeedProjectsWithStatusAsync(companies[1], ProjectStatus.Complete, 2);
        
        // Seed students for applications
        var students = await SeedStudentsAsync(3);
        
        // Combine all projects for application seeding
        var allProjects = activeProjects.Concat(completedProjects).ToList();
        
        // Seed 5 Applications (Status = ApplicationStatus.Pending)
        await SeedApplicationsWithStatusAsync(students, allProjects, ApplicationStatus.Pending, 5);
        
        // Seed 4 Applications (Status = ApplicationStatus.Accepted)
        await SeedApplicationsWithStatusAsync(students, allProjects, ApplicationStatus.Accepted, 4);
        
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dashboardmetrics/platform-overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PlatformOverviewDto>();
        result.Should().NotBeNull();
        
        // Assert the project status counts match
        result!.ActiveProjects.Should().Be(3, "we seeded 3 active projects");
        result.CompletedProjects.Should().Be(2, "we seeded 2 completed projects");
        
        // Assert the application status counts match
        result.PendingApplications.Should().Be(5, "we seeded 5 pending applications");
        result.AcceptedApplications.Should().Be(4, "we seeded 4 accepted applications");
        result.TotalApplications.Should().Be(9, "5 pending + 4 accepted = 9 total");
    }

    [Fact]
    public async Task GetPlatformOverview_ReturnsCorrectAveragesAndPending()
    {
        // Arrange - Clear database first to ensure clean state
        await ClearDatabaseAsync();
        
        // Arrange - Seed an Admin user
        var adminUser = await SeedAdminUserAsync();
        
        // Seed companies and students
        var companies = await SeedCompaniesAsync(2);
        var students = await SeedStudentsAsync(2);
        
        // Seed 2 Approved Company Reviews with Rating = 5.0
        await SeedCompanyReviewsWithRatingAsync(companies[0], students[0], rating: 5.0m, status: ReviewStatus.Approved, count: 2);
        
        // Seed 1 Approved Student Review with Rating = 3.0
        await SeedStudentReviewsWithRatingAsync(students[1], companies[1], rating: 3.0m, status: ReviewStatus.Approved, count: 1);
        
        // Seed 4 Pending Reviews (2 Company + 2 Student)
        await SeedCompanyReviewsWithRatingAsync(companies[1], students[1], rating: 4.0m, status: ReviewStatus.Pending, count: 2);
        await SeedStudentReviewsWithRatingAsync(students[0], companies[0], rating: 4.5m, status: ReviewStatus.Pending, count: 2);
        
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dashboardmetrics/platform-overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PlatformOverviewDto>();
        result.Should().NotBeNull();
        
        // Assert average ratings (only approved reviews count)
        result!.AverageCompanyRating.Should().Be(5.0m, "2 approved company reviews both have rating 5.0");
        result.AverageStudentRating.Should().Be(3.0m, "1 approved student review has rating 3.0");
        
        // Assert total reviews (only approved count)
        result.TotalReviews.Should().Be(3, "2 approved company reviews + 1 approved student review");
    }

    #region Helper Methods

    /// <summary>
    /// Clears all database tables to ensure clean state between tests
    /// </summary>
    private async Task ClearDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Remove all entities using EF Core (respects relationships)
        dbContext.Certificates.RemoveRange(dbContext.Certificates);
        dbContext.StudentReviews.RemoveRange(dbContext.StudentReviews);
        dbContext.CompanyReviews.RemoveRange(dbContext.CompanyReviews);
        dbContext.Applications.RemoveRange(dbContext.Applications);
        dbContext.Projects.RemoveRange(dbContext.Projects);
        dbContext.Students.RemoveRange(dbContext.Students);
        dbContext.Companies.RemoveRange(dbContext.Companies);
        dbContext.Users.RemoveRange(dbContext.Users);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Seeds an admin user for testing
    /// </summary>
    private async Task<User> SeedAdminUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var adminUser = TestDataGenerator.CreateTestUser(UserType.Admin, $"admin-{Guid.NewGuid()}@test.com");
        await dbContext.Users.AddAsync(adminUser);
        await dbContext.SaveChangesAsync();
        
        dbContext.ChangeTracker.Clear();
        return adminUser;
    }

    /// <summary>
    /// Seeds a regular student user for testing
    /// </summary>
    private async Task<User> SeedStudentUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await dbContext.Users.AddAsync(studentUser);
        await dbContext.SaveChangesAsync();
        
        dbContext.ChangeTracker.Clear();
        return studentUser;
    }

    /// <summary>
    /// Seeds multiple student users with profiles
    /// </summary>
    private async Task<List<Student>> SeedStudentsAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var students = new List<Student>();
        
        for (int i = 0; i < count; i++)
        {
            var user = TestDataGenerator.CreateTestUser(UserType.Student, $"student-{Guid.NewGuid()}@test.com");
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var student = TestDataGenerator.CreateTestStudent(user.UserID);
            student.Status = StudentStatus.Active; // Ensure active for counting
            await dbContext.Students.AddAsync(student);
            students.Add(student);
        }

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        
        return students;
    }

    /// <summary>
    /// Seeds multiple company users with profiles
    /// </summary>
    private async Task<List<Company>> SeedCompaniesAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var companies = new List<Company>();
        
        for (int i = 0; i < count; i++)
        {
            var user = TestDataGenerator.CreateTestUser(UserType.Company, $"company-{Guid.NewGuid()}@test.com");
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var company = TestDataGenerator.CreateTestCompany(user.UserID);
            await dbContext.Companies.AddAsync(company);
            companies.Add(company);
        }

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        
        return companies;
    }

    /// <summary>
    /// Seeds multiple projects with various statuses (6 Active, 2 Complete, 2 Draft)
    /// </summary>
    private async Task<List<Project>> SeedProjectsAsync(List<Company> companies, int totalCount)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var projects = new List<Project>();
        
        // Create 6 Active projects
        for (int i = 0; i < 6; i++)
        {
            var company = companies[i % companies.Count];
            var project = TestDataGenerator.CreateTestProject(company.CompanyID, company.UserID);
            project.Status = ProjectStatus.Active;
            projects.Add(project);
        }
        
        // Create 2 Complete projects
        for (int i = 0; i < 2; i++)
        {
            var company = companies[i % companies.Count];
            var project = TestDataGenerator.CreateTestProject(company.CompanyID, company.UserID);
            project.Status = ProjectStatus.Complete;
            projects.Add(project);
        }
        
        // Create 2 Draft projects
        for (int i = 0; i < 2; i++)
        {
            var company = companies[i % companies.Count];
            var project = TestDataGenerator.CreateTestProject(company.CompanyID, company.UserID);
            project.Status = ProjectStatus.Draft;
            projects.Add(project);
        }

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        
        return projects;
    }

    /// <summary>
    /// Seeds projects with a specific status
    /// </summary>
    private async Task<List<Project>> SeedProjectsWithStatusAsync(Company company, ProjectStatus status, int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var projects = new List<Project>();
        
        for (int i = 0; i < count; i++)
        {
            var project = TestDataGenerator.CreateTestProject(company.CompanyID, company.UserID);
            project.Status = status;
            projects.Add(project);
        }

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        
        return projects;
    }

    /// <summary>
    /// Seeds multiple applications with various statuses (10 Pending, 8 Accepted, 7 Rejected)
    /// </summary>
    private async Task<List<Domain.Entities.Application>> SeedApplicationsAsync(
        List<Student> students, 
        List<Project> projects, 
        int totalCount)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var applications = new List<Domain.Entities.Application>();
        
        // Get existing combinations to avoid duplicates
        var existingCombinations = await dbContext.Applications
            .Select(a => $"{a.StudentID}-{a.ProjectID}")
            .ToListAsync();
        var usedCombinations = new HashSet<string>(existingCombinations);
        
        int studentIndex = 0;
        int projectIndex = 0;
        
        // Create 10 Pending applications
        while (applications.Count(a => a.Status == ApplicationStatus.Pending) < 10 
               && usedCombinations.Count < students.Count * projects.Count)
        {
            var student = students[studentIndex % students.Count];
            var project = projects[projectIndex % projects.Count];
            var combination = $"{student.StudentID}-{project.ProjectID}";
            
            if (!usedCombinations.Contains(combination))
            {
                usedCombinations.Add(combination);
                var application = new Domain.Entities.Application
                {
                    ProjectID = project.ProjectID,
                    StudentID = student.StudentID,
                    Status = ApplicationStatus.Pending,
                    Resume = "resume.pdf",
                    CoverLetter = $"Test cover letter {applications.Count}",
                    AppliedAt = DateTime.UtcNow.AddDays(-applications.Count)
                };
                applications.Add(application);
            }
            
            projectIndex++;
            if (projectIndex >= projects.Count)
            {
                projectIndex = 0;
                studentIndex++;
            }
        }
        
        // Create 8 Accepted applications
        while (applications.Count(a => a.Status == ApplicationStatus.Accepted) < 8 
               && usedCombinations.Count < students.Count * projects.Count)
        {
            var student = students[studentIndex % students.Count];
            var project = projects[projectIndex % projects.Count];
            var combination = $"{student.StudentID}-{project.ProjectID}";
            
            if (!usedCombinations.Contains(combination))
            {
                usedCombinations.Add(combination);
                var application = new Domain.Entities.Application
                {
                    ProjectID = project.ProjectID,
                    StudentID = student.StudentID,
                    Status = ApplicationStatus.Accepted,
                    Resume = "resume.pdf",
                    CoverLetter = $"Test cover letter accepted {applications.Count}",
                    AppliedAt = DateTime.UtcNow.AddDays(-applications.Count - 10)
                };
                applications.Add(application);
            }
            
            projectIndex++;
            if (projectIndex >= projects.Count)
            {
                projectIndex = 0;
                studentIndex++;
            }
        }
        
        // Create 7 Rejected applications
        while (applications.Count(a => a.Status == ApplicationStatus.Rejected) < 7 
               && usedCombinations.Count < students.Count * projects.Count)
        {
            var student = students[studentIndex % students.Count];
            var project = projects[projectIndex % projects.Count];
            var combination = $"{student.StudentID}-{project.ProjectID}";
            
            if (!usedCombinations.Contains(combination))
            {
                usedCombinations.Add(combination);
                var application = new Domain.Entities.Application
                {
                    ProjectID = project.ProjectID,
                    StudentID = student.StudentID,
                    Status = ApplicationStatus.Rejected,
                    Resume = "resume.pdf",
                    CoverLetter = $"Test cover letter rejected {applications.Count}",
                    AppliedAt = DateTime.UtcNow.AddDays(-applications.Count - 20)
                };
                applications.Add(application);
            }
            
            projectIndex++;
            if (projectIndex >= projects.Count)
            {
                projectIndex = 0;
                studentIndex++;
            }
        }

        if (applications.Any())
        {
            await dbContext.Applications.AddRangeAsync(applications);
            await dbContext.SaveChangesAsync();
        }
        
        dbContext.ChangeTracker.Clear();
        
        return applications;
    }

    /// <summary>
    /// Seeds applications with a specific status
    /// </summary>
    private async Task<List<Domain.Entities.Application>> SeedApplicationsWithStatusAsync(
        List<Student> students,
        List<Project> projects,
        ApplicationStatus status,
        int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var applications = new List<Domain.Entities.Application>();
        
        // Get existing combinations to avoid duplicates
        var existingCombinations = await dbContext.Applications
            .Select(a => $"{a.StudentID}-{a.ProjectID}")
            .ToListAsync();
        var usedCombinations = new HashSet<string>(existingCombinations);
        
        int studentIndex = 0;
        int projectIndex = 0;
        
        while (applications.Count < count && usedCombinations.Count < students.Count * projects.Count)
        {
            var student = students[studentIndex % students.Count];
            var project = projects[projectIndex % projects.Count];
            var combination = $"{student.StudentID}-{project.ProjectID}";
            
            if (!usedCombinations.Contains(combination))
            {
                usedCombinations.Add(combination);
                
                var application = new Domain.Entities.Application
                {
                    ProjectID = project.ProjectID,
                    StudentID = student.StudentID,
                    Status = status,
                    Resume = "resume.pdf",
                    CoverLetter = $"Test cover letter {status} {applications.Count}",
                    AppliedAt = DateTime.UtcNow.AddDays(-applications.Count)
                };
                applications.Add(application);
            }
            
            // Move to next combination
            projectIndex++;
            if (projectIndex >= projects.Count)
            {
                projectIndex = 0;
                studentIndex++;
            }
        }

        if (applications.Any())
        {
            await dbContext.Applications.AddRangeAsync(applications);
            await dbContext.SaveChangesAsync();
        }
        
        dbContext.ChangeTracker.Clear();
        
        return applications;
    }

    /// <summary>
    /// Seeds company reviews with specified approved and pending counts
    /// </summary>
    private async Task SeedCompanyReviewsAsync(
        List<Company> companies,
        List<Student> students,
        int approvedCount,
        int pendingCount)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var reviews = new List<CompanyReview>();
        
        // Create approved reviews
        for (int i = 0; i < approvedCount; i++)
        {
            var company = companies[i % companies.Count];
            var student = students[i % students.Count];
            
            var review = TestDataGenerator.CreateTestCompanyReview(company.CompanyID, student.StudentID);
            review.Status = ReviewStatus.Approved;
            review.Rating = 4.5m; // Set a consistent rating
            reviews.Add(review);
        }
        
        // Create pending reviews
        for (int i = 0; i < pendingCount; i++)
        {
            var company = companies[i % companies.Count];
            var student = students[i % students.Count];
            
            var review = TestDataGenerator.CreateTestCompanyReview(company.CompanyID, student.StudentID);
            review.Status = ReviewStatus.Pending;
            review.Rating = 4.0m;
            reviews.Add(review);
        }

        await dbContext.CompanyReviews.AddRangeAsync(reviews);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Seeds company reviews with specific rating and status
    /// </summary>
    private async Task SeedCompanyReviewsWithRatingAsync(
        Company company,
        Student student,
        decimal rating,
        ReviewStatus status,
        int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var reviews = new List<CompanyReview>();
        
        for (int i = 0; i < count; i++)
        {
            var review = TestDataGenerator.CreateTestCompanyReview(company.CompanyID, student.StudentID);
            review.Status = status;
            review.Rating = rating;
            review.CreatedAt = DateTime.UtcNow.AddDays(-i); // Different dates to avoid duplicates
            reviews.Add(review);
        }

        await dbContext.CompanyReviews.AddRangeAsync(reviews);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Seeds student reviews with specified approved and pending counts
    /// </summary>
    private async Task SeedStudentReviewsAsync(
        List<Student> students,
        List<Company> companies,
        int approvedCount,
        int pendingCount)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var reviews = new List<StudentReview>();
        
        // Create approved reviews
        for (int i = 0; i < approvedCount; i++)
        {
            var student = students[i % students.Count];
            var company = companies[i % companies.Count];
            
            var review = TestDataGenerator.CreateTestStudentReview(student.StudentID, company.CompanyID);
            review.Status = ReviewStatus.Approved;
            review.Rating = 3.8m; // Set a consistent rating
            reviews.Add(review);
        }
        
        // Create pending reviews
        for (int i = 0; i < pendingCount; i++)
        {
            var student = students[i % students.Count];
            var company = companies[i % companies.Count];
            
            var review = TestDataGenerator.CreateTestStudentReview(student.StudentID, company.CompanyID);
            review.Status = ReviewStatus.Pending;
            review.Rating = 3.5m;
            reviews.Add(review);
        }

        await dbContext.StudentReviews.AddRangeAsync(reviews);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Seeds student reviews with specific rating and status
    /// </summary>
    private async Task SeedStudentReviewsWithRatingAsync(
        Student student,
        Company company,
        decimal rating,
        ReviewStatus status,
        int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var reviews = new List<StudentReview>();
        
        for (int i = 0; i < count; i++)
        {
            var review = TestDataGenerator.CreateTestStudentReview(student.StudentID, company.CompanyID);
            review.Status = status;
            review.Rating = rating;
            review.CreatedAt = DateTime.UtcNow.AddDays(-i); // Different dates to avoid duplicates
            reviews.Add(review);
        }

        await dbContext.StudentReviews.AddRangeAsync(reviews);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Seeds certificates for testing
    /// </summary>
    private async Task SeedCertificatesAsync(
        List<Company> companies,
        List<Student> students,
        int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var certificates = new List<Certificate>();
        
        for (int i = 0; i < count; i++)
        {
            var company = companies[i % companies.Count];
            var student = students[i % students.Count];
            
            // Need to get a project from the company
            var project = await dbContext.Projects
                .FirstOrDefaultAsync(p => p.CompanyID == company.CompanyID);
            
            // If no project exists, create one
            if (project == null)
            {
                project = TestDataGenerator.CreateTestProject(company.CompanyID, company.UserID);
                await dbContext.Projects.AddAsync(project);
                await dbContext.SaveChangesAsync();
            }
            
            var certificate = new Certificate
            {
                CompanyID = company.CompanyID,
                StudentID = student.StudentID,
                ProjectID = project.ProjectID,
                CertificateNumber = Guid.NewGuid().ToString(),
                CertificateTitle = $"Test Certificate {i + 1}",
                Description = $"Certificate description {i + 1}",
                IssuedAt = DateTime.UtcNow.AddDays(-i),
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            };
            certificates.Add(certificate);
        }

        await dbContext.Certificates.AddRangeAsync(certificates);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
