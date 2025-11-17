using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Domain.Entities;
using Sh8lny.IntegrationTests.Fixtures;
using Sh8lny.IntegrationTests.Helpers;
using Sh8lny.Persistence.Contexts;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Sh8lny.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for ReviewsController - Moderation Actions
/// </summary>
public class ReviewsControllerModerationTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReviewsControllerModerationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ApproveCompanyReview_AsAdmin_ApprovesSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify status changed using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.CompanyReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Approved);
    }

    [Fact]
    public async Task ApproveStudentReview_AsAdmin_ApprovesSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Pending);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=student", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.StudentReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Approved);
    }

    [Fact]
    public async Task ApproveReview_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        // Use regular student token (not admin)
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ApproveReview_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        // Act - No authorization header
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RejectCompanyReview_AsAdmin_RejectsSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/reject?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.CompanyReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Rejected);
    }

    [Fact]
    public async Task RejectStudentReview_AsAdmin_RejectsSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Pending);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/reject?reviewType=student", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.StudentReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Rejected);
    }

    [Fact]
    public async Task RejectReview_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/reject?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task FlagCompanyReview_AsAuthenticatedUser_FlagsSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved);

        // Any authenticated user can flag
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/flag?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.CompanyReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Flagged);
    }

    [Fact]
    public async Task FlagStudentReview_AsAuthenticatedUser_FlagsSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/flag?reviewType=student", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedReview = await db.StudentReviews.FindAsync(review.ReviewID);
        updatedReview.Should().NotBeNull();
        updatedReview!.Status.Should().Be(ReviewStatus.Flagged);
    }

    [Fact]
    public async Task FlagPendingReview_ReturnsBadRequest()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Can only flag approved reviews
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/flag?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FlagReview_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved);

        // Act - No authorization header
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/flag?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ApproveReview_WithInvalidReviewType_ReturnsBadRequest()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Invalid review type
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=invalid", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ApproveReview_WithNonExistentReviewId_ReturnsNotFound()
    {
        // Arrange
        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/api/reviews/99999/approve?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApproveCompanyReview_UpdatesCompanyRatingStatistics()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create pending review
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending, rating: 5m);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify company rating was updated using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedCompany = await db.Companies.FindAsync(company.CompanyID);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.AverageRating.Should().Be(5m);
        updatedCompany.TotalReviews.Should().Be(1);
    }

    [Fact]
    public async Task ApproveStudentReview_UpdatesStudentRatingStatistics()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create pending public review
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Pending, isPublic: true, rating: 4.5m);

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/approve?reviewType=student", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify student rating was updated using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedStudent = await db.Students.FindAsync(student.StudentID);
        updatedStudent.Should().NotBeNull();
        updatedStudent!.AverageRating.Should().Be(4.5m);
        updatedStudent.TotalReviews.Should().Be(1);
    }

    [Fact]
    public async Task RejectCompanyReview_RemovesFromCompanyRating()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create approved review first
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved, rating: 5m);
        
        // Update company stats manually (simulating previous approval)
        using (var setupScope = _factory.Services.CreateScope())
        {
            var setupDb = setupScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            var companyToUpdate = await setupDb.Companies.FindAsync(company.CompanyID);
            companyToUpdate!.AverageRating = 5m;
            companyToUpdate.TotalReviews = 1;
            setupDb.Companies.Update(companyToUpdate);
            await setupDb.SaveChangesAsync();
        }

        var adminUser = await SeedAdminUserAsync();
        var token = JwtTokenHelper.GenerateAdminToken(adminUser.UserID, adminUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Reject the review
        var response = await _client.PostAsync($"/api/reviews/{review.ReviewID}/reject?reviewType=company", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify company rating was recalculated (should be 0 now) using fresh scope
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var updatedCompany = await verifyDb.Companies.FindAsync(company.CompanyID);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.AverageRating.Should().Be(0m);
        updatedCompany.TotalReviews.Should().Be(0);
    }

    #region Helper Methods

    private async Task<(User studentUser, Student student, User companyUser, Company company)> SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await dbContext.Users.AddAsync(studentUser);
        await dbContext.SaveChangesAsync();

        var student = TestDataGenerator.CreateTestStudent(studentUser.UserID);
        await dbContext.Students.AddAsync(student);

        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await dbContext.Users.AddAsync(companyUser);
        await dbContext.SaveChangesAsync();

        var company = TestDataGenerator.CreateTestCompany(companyUser.UserID);
        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();
        
        // Clear change tracker to force controller's scope to query fresh
        dbContext.ChangeTracker.Clear();

        return (studentUser, student, companyUser, company);
    }

    private async Task<User> SeedAdminUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        // Generate unique admin email for each test to avoid UNIQUE constraint violations
        var adminUser = TestDataGenerator.CreateTestUser(UserType.Admin);
        await dbContext.Users.AddAsync(adminUser);
        await dbContext.SaveChangesAsync();
        return adminUser;
    }

    private async Task<CompanyReview> SeedCompanyReviewAsync(
        int companyId,
        int studentId,
        ReviewStatus status,
        decimal rating = 4.5m)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var review = TestDataGenerator.CreateTestCompanyReview(companyId, studentId);
        review.Status = status;
        review.Rating = rating;
        
        await dbContext.CompanyReviews.AddAsync(review);
        await dbContext.SaveChangesAsync();
        return review;
    }

    private async Task<StudentReview> SeedStudentReviewAsync(
        int studentId,
        int companyId,
        ReviewStatus status,
        bool isPublic = true,
        decimal rating = 4.5m)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var review = TestDataGenerator.CreateTestStudentReview(studentId, companyId);
        review.Status = status;
        review.IsPublic = isPublic;
        review.Rating = rating;
        
        await dbContext.StudentReviews.AddAsync(review);
        await dbContext.SaveChangesAsync();
        return review;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
