using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.ActivityLogs;
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
/// Integration tests for ActivityLogsController
/// </summary>
public class ActivityLogControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ActivityLogControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetRecentActivity_AsAdmin_ReturnsLogs()
    {
        // Arrange - Seed admin user and activity logs
        var adminUser = await SeedAdminUserAsync();
        var (user1, user2, logs) = await SeedActivityLogsAsync();

        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/activitylogs/recent?count=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActivityLogDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterOrEqualTo(logs.Count);
        
        // Verify logs contain expected data
        result.Should().Contain(log => log.ActivityType == "ReviewApproved");
        result.Should().Contain(log => log.ActivityType == "CertificateIssued");
    }

    [Fact]
    public async Task GetRecentActivity_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange - Seed regular student user
        var studentUser = await SeedStudentUserAsync();
        
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/activitylogs/recent?count=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetLogsForUser_AsAdmin_ReturnsUserSpecificLogs()
    {
        // Arrange - Seed admin and multiple users with logs
        var adminUser = await SeedAdminUserAsync();
        var (targetUser, otherUser, logs) = await SeedActivityLogsAsync();

        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get logs for targetUser (should have 3 logs)
        var targetUserLogs = logs.Where(l => l.UserID == targetUser.UserID).ToList();
        targetUserLogs.Count.Should().Be(3);

        // Act
        var response = await _client.GetAsync($"/api/activitylogs/user/{targetUser.UserID}?page=1&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActivityLogDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().Be(3);
        
        // Verify all logs belong to the target user
        result.All(log => log.UserID == targetUser.UserID).Should().BeTrue();
        
        // Verify other user's logs are not included
        result.Should().NotContain(log => log.UserID == otherUser.UserID);
    }

    [Fact]
    public async Task Integration_WhenReviewApproved_LogIsCreated()
    {
        // Arrange - Seed all necessary data for a review approval
        var (studentUser, student, companyUser, company) = await SeedReviewTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);
        
        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Approve the review
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=company", null);

        // Assert - Review approval succeeded
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify - Activity log was created
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var activityLog = await db.ActivityLogs
            .Where(log => log.ActivityType == "ReviewApproved" 
                       && log.RelatedEntityType == "CompanyReview" 
                       && log.RelatedEntityID == review.ReviewID)
            .FirstOrDefaultAsync();

        activityLog.Should().NotBeNull();
        activityLog!.UserID.Should().BePositive("activity log should have a valid UserID");
        activityLog.Description.Should().Contain("approved");
        activityLog.Description.Should().Contain(review.ReviewID.ToString());
    }

    [Fact]
    public async Task GetRecentActivity_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act - No authorization header
        var response = await _client.GetAsync("/api/activitylogs/recent?count=20");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserLogs_AsRegularUser_ReturnsOwnLogs()
    {
        // Arrange - Seed student user with their own logs
        var studentUser = await SeedStudentUserAsync();
        var logs = await SeedActivityLogsForUserAsync(studentUser.UserID, 3);
        
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Regular user accessing their own logs
        var response = await _client.GetAsync($"/api/activitylogs/user/{studentUser.UserID}?page=1&pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ActivityLogDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().Be(3);
        result.All(log => log.UserID == studentUser.UserID).Should().BeTrue();
    }

    [Fact]
    public async Task GetActivityStats_AsAdmin_ReturnsStatistics()
    {
        // Arrange - Seed admin and activity logs
        var adminUser = await SeedAdminUserAsync();
        await SeedActivityLogsAsync();

        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/activitylogs/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ActivityStatsDto>();
        result.Should().NotBeNull();
        result!.TotalActivities.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetActivityStats_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange
        var studentUser = await SeedStudentUserAsync();
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/activitylogs/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #region Helper Methods

    /// <summary>
    /// Seeds an admin user for testing
    /// </summary>
    private async Task<User> SeedAdminUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var adminUser = TestDataGenerator.CreateTestUser(UserType.Admin);
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
    /// Seeds activity logs for multiple users
    /// </summary>
    private async Task<(User user1, User user2, List<ActivityLog> logs)> SeedActivityLogsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create two test users
        var user1 = TestDataGenerator.CreateTestUser(UserType.Student);
        var user2 = TestDataGenerator.CreateTestUser(UserType.Company);
        await dbContext.Users.AddAsync(user1);
        await dbContext.Users.AddAsync(user2);
        await dbContext.SaveChangesAsync();

        var logs = new List<ActivityLog>();

        // Create 3 logs for user1
        var log1 = new ActivityLog
        {
            UserID = user1.UserID,
            ActivityType = "ReviewApproved",
            Description = "Admin approved company review 1",
            RelatedEntityType = "CompanyReview",
            RelatedEntityID = 1,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30)
        };
        logs.Add(log1);

        var log2 = new ActivityLog
        {
            UserID = user1.UserID,
            ActivityType = "CertificateIssued",
            Description = "Company issued certificate to student",
            RelatedEntityType = "Certificate",
            RelatedEntityID = 1,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20)
        };
        logs.Add(log2);

        var log3 = new ActivityLog
        {
            UserID = user1.UserID,
            ActivityType = "ApplicationSubmitted",
            Description = "Student applied to project",
            RelatedEntityType = "Application",
            RelatedEntityID = 1,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        logs.Add(log3);

        // Create 2 logs for user2
        var log4 = new ActivityLog
        {
            UserID = user2.UserID,
            ActivityType = "ReviewApproved",
            Description = "Admin approved student review 2",
            RelatedEntityType = "StudentReview",
            RelatedEntityID = 2,
            CreatedAt = DateTime.UtcNow.AddMinutes(-15)
        };
        logs.Add(log4);

        var log5 = new ActivityLog
        {
            UserID = user2.UserID,
            ActivityType = "CertificateRevoked",
            Description = "Company revoked certificate",
            RelatedEntityType = "Certificate",
            RelatedEntityID = 2,
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        logs.Add(log5);

        await dbContext.ActivityLogs.AddRangeAsync(logs);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        return (user1, user2, logs);
    }

    /// <summary>
    /// Seeds activity logs for a specific user
    /// </summary>
    private async Task<List<ActivityLog>> SeedActivityLogsForUserAsync(int userId, int count)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var logs = new List<ActivityLog>();

        for (int i = 0; i < count; i++)
        {
            var log = new ActivityLog
            {
                UserID = userId,
                ActivityType = i % 2 == 0 ? "ApplicationSubmitted" : "ReviewApproved",
                Description = $"Test activity log {i + 1}",
                RelatedEntityType = i % 2 == 0 ? "Application" : "Review",
                RelatedEntityID = i + 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i * 5)
            };
            logs.Add(log);
        }

        await dbContext.ActivityLogs.AddRangeAsync(logs);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        return logs;
    }

    /// <summary>
    /// Seeds test data for review approval tests
    /// </summary>
    private async Task<(User studentUser, Student student, User companyUser, Company company)> SeedReviewTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create student user and profile
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await dbContext.Users.AddAsync(studentUser);
        await dbContext.SaveChangesAsync();

        var student = TestDataGenerator.CreateTestStudent(studentUser.UserID);
        await dbContext.Students.AddAsync(student);
        await dbContext.SaveChangesAsync();

        // Create company user and profile
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await dbContext.Users.AddAsync(companyUser);
        await dbContext.SaveChangesAsync();

        var company = TestDataGenerator.CreateTestCompany(companyUser.UserID);
        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        return (studentUser, student, companyUser, company);
    }

    /// <summary>
    /// Seeds a company review for testing
    /// </summary>
    private async Task<CompanyReview> SeedCompanyReviewAsync(
        int companyId,
        int studentId,
        ReviewStatus status)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        var review = TestDataGenerator.CreateTestCompanyReview(companyId, studentId);
        review.Status = status;

        await dbContext.CompanyReviews.AddAsync(review);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        return review;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
