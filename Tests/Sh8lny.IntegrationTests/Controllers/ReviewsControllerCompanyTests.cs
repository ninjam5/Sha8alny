using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.Reviews;
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
/// Integration tests for ReviewsController - Company Reviews
/// </summary>
public class ReviewsControllerCompanyTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReviewsControllerCompanyTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Note: Database schema is created once by IAsyncLifetime.InitializeAsync()
        // Each test seeds its own data inline, so no need to clear or recreate schema here
    }

    [Fact]
    public async Task CreateCompanyReview_WithValidData_ReturnsCreatedReview()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyReviewDto
        {
            CompanyID = company.CompanyID,
            StudentID = student.StudentID,
            Rating = 4.5m,
            ReviewTitle = "Great Experience",
            ReviewText = "I had an amazing internship at this company.",
            WorkEnvironmentRating = 5m,
            LearningOpportunityRating = 4.5m,
            MentorshipRating = 4m,
            WouldRecommend = true,
            Pros = "Great team, modern tech stack",
            Cons = "Office location",
            IsAnonymous = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var review = await response.Content.ReadFromJsonAsync<CompanyReviewDto>();
        review.Should().NotBeNull();
        review!.ReviewID.Should().BeGreaterThan(0);
        review.Rating.Should().Be(4.5m);
        review.Status.Should().Be("Pending");
        review.CompanyName.Should().Be(company.CompanyName);
        review.IsAnonymous.Should().BeFalse();
    }

    [Fact]
    public async Task CreateCompanyReview_WithAnonymousFlag_HidesStudentName()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyReviewDto
        {
            CompanyID = company.CompanyID,
            StudentID = student.StudentID,
            Rating = 3m,
            ReviewText = "Anonymous feedback",
            WouldRecommend = false,
            IsAnonymous = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var review = await response.Content.ReadFromJsonAsync<CompanyReviewDto>();
        review.Should().NotBeNull();
        review!.StudentName.Should().BeNull();
        review.IsAnonymous.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompanyReview_WithInvalidRating_ReturnsBadRequest()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyReviewDto
        {
            CompanyID = company.CompanyID,
            StudentID = student.StudentID,
            Rating = 6m, // Invalid - must be 1-5
            ReviewText = "Test",
            WouldRecommend = true,
            IsAnonymous = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCompanyReview_DuplicateForSameOpportunity_ReturnsConflict()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var opportunity = await SeedCompletedOpportunityAsync(student.StudentID, company.CompanyID);
        
        // Create first review using fresh scope
        using (var setupScope = _factory.Services.CreateScope())
        {
            var setupDb = setupScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            await setupDb.CompanyReviews.AddAsync(new CompanyReview
            {
                CompanyID = company.CompanyID,
                StudentID = student.StudentID,
                CompletedOpportunityID = opportunity.CompletedOpportunityID,
                Rating = 4m,
                WouldRecommend = true,
                Status = ReviewStatus.Approved,
                IsVerified = true,
                IsAnonymous = false,
                CreatedAt = DateTime.UtcNow
            });
            await setupDb.SaveChangesAsync();
        }

        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyReviewDto
        {
            CompanyID = company.CompanyID,
            StudentID = student.StudentID,
            CompletedOpportunityID = opportunity.CompletedOpportunityID,
            Rating = 5m,
            WouldRecommend = true,
            IsAnonymous = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCompanyReviewById_WithValidId_ReturnsReview()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID);

        // Act - Public endpoint, no auth required
        var response = await _client.GetAsync($"/api/reviews/companies/{review.ReviewID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviewDto = await response.Content.ReadFromJsonAsync<CompanyReviewDto>();
        reviewDto.Should().NotBeNull();
        reviewDto!.ReviewID.Should().Be(review.ReviewID);
        reviewDto.CompanyID.Should().Be(company.CompanyID);
    }

    [Fact]
    public async Task GetCompanyReviewById_WithNonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/reviews/companies/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCompanyReviews_ReturnsPaginatedApprovedReviews()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create multiple reviews with different statuses
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Rejected);

        // Act
        var response = await _client.GetAsync($"/api/reviews/companies/by-company/{company.CompanyID}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<CompanyReviewDto>>();
        reviews.Should().NotBeNull();
        reviews!.Count.Should().Be(2); // Only approved reviews
        reviews.All(r => r.Status == "Approved").Should().BeTrue();
    }

    [Fact]
    public async Task GetStudentCompanyReviews_ReturnsAllStudentReviews()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var (_, _, company2User, company2) = await SeedTestDataAsync();
        
        // Student reviews multiple companies
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID);
        await SeedCompanyReviewAsync(company2.CompanyID, student.StudentID);

        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/reviews/companies/by-student/{student.StudentID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<CompanyReviewDto>>();
        reviews.Should().NotBeNull();
        reviews!.Count.Should().Be(2);
        reviews.All(r => r.StudentID == student.StudentID).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCompanyReview_AsOwner_UpdatesSuccessfully()
    {
        // Arrange - Seed data inline within explicit scope
        User studentUser, companyUser;
        Student student;
        Company company;
        CompanyReview review;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            // Create student user and entity
            studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(studentUser);
            await db.SaveChangesAsync();

            student = TestDataGenerator.CreateTestStudent(studentUser.UserID);
            await db.Students.AddAsync(student);
            await db.SaveChangesAsync();

            // Create company user and entity
            companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
            await db.Users.AddAsync(companyUser);
            await db.SaveChangesAsync();

            company = TestDataGenerator.CreateTestCompany(companyUser.UserID);
            await db.Companies.AddAsync(company);
            await db.SaveChangesAsync();

            // Create project
            var project = TestDataGenerator.CreateTestProject(company.CompanyID, companyUser.UserID);
            await db.Projects.AddAsync(project);
            await db.SaveChangesAsync();

            // Create completed opportunity
            var completedOpportunity = TestDataGenerator.CreateTestCompletedOpportunity(student.StudentID, company.CompanyID, project.ProjectID);
            await db.CompletedOpportunities.AddAsync(completedOpportunity);
            await db.SaveChangesAsync();

            // Create company review
            review = TestDataGenerator.CreateTestCompanyReview(company.CompanyID, student.StudentID);
            review.Status = ReviewStatus.Approved;
            review.Rating = 4.5m;
            review.WouldRecommend = true;
            await db.CompanyReviews.AddAsync(review);
            await db.SaveChangesAsync();
        } // Scope disposed - data committed to SQLite connection

        // Generate token and make HTTP request
        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCompanyReviewDto
        {
            ReviewID = review.ReviewID,
            Rating = 5m,
            ReviewTitle = "Updated Title",
            ReviewText = "Updated review text"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/companies/{review.ReviewID}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CompanyReviewDto>();
        updated.Should().NotBeNull();
        updated!.Rating.Should().Be(5m);
        updated.ReviewTitle.Should().Be("Updated Title");
        updated.Status.Should().Be("Pending"); // Reset to pending after update
        
        // Verify in database using fresh scope
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var dbReview = await verifyDb.CompanyReviews.FindAsync(review.ReviewID);
        dbReview.Should().NotBeNull();
        dbReview!.Rating.Should().Be(5m);
    }

    [Fact]
    public async Task UpdateCompanyReview_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var (otherStudentUser, otherStudent, _, _) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID);

        // Try to update with different student's token
        var token = JwtTokenHelper.GenerateStudentToken(otherStudentUser.UserID, otherStudentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateCompanyReviewDto
        {
            ReviewID = review.ReviewID,
            Rating = 5m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/companies/{review.ReviewID}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCompanyReview_AsOwner_DeletesSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID);

        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/reviews/companies/{review.ReviewID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify deletion using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var deletedReview = await db.CompanyReviews.FindAsync(review.ReviewID);
        deletedReview.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCompanyReview_AsNonOwner_ReturnsForbidden()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var (otherStudentUser, _, _, _) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID);

        var token = JwtTokenHelper.GenerateStudentToken(otherStudentUser.UserID, otherStudentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/reviews/companies/{review.ReviewID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddCompanyResponse_AsCompany_AddsResponseSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseDto = new CompanyResponseDto
        {
            ReviewID = review.ReviewID,
            CompanyResponse = "Thank you for your feedback!"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reviews/companies/{review.ReviewID}/response", responseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CompanyReviewDto>();
        updated.Should().NotBeNull();
        updated!.CompanyResponse.Should().Be("Thank you for your feedback!");
        updated.CompanyRespondedAt.Should().NotBeNull();
        
        // Verify in database using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var dbReview = await db.CompanyReviews.FindAsync(review.ReviewID);
        dbReview.Should().NotBeNull();
        dbReview!.CompanyResponse.Should().Be("Thank you for your feedback!");
    }

    [Fact]
    public async Task AddCompanyResponse_ToPendingReview_ReturnsBadRequest()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseDto = new CompanyResponseDto
        {
            ReviewID = review.ReviewID,
            CompanyResponse = "Trying to respond to pending review"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reviews/companies/{review.ReviewID}/response", responseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCompanyReviewStats_ReturnsAccurateStatistics()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create reviews with known ratings
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved, 5m, true);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved, 4.5m, true);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Approved, 3m, false);
        await SeedCompanyReviewAsync(company.CompanyID, student.StudentID, ReviewStatus.Pending, 5m, true); // Not counted

        // Act
        var response = await _client.GetAsync($"/api/reviews/companies/stats/{company.CompanyID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<ReviewStatsDto>();
        stats.Should().NotBeNull();
        stats!.TotalReviews.Should().Be(3);
        stats.AverageRating.Should().BeApproximately(4.17m, 0.01m);
        stats.FiveStarCount.Should().Be(2); // 5 and 4.5
        stats.ThreeStarCount.Should().Be(1); // 3
        stats.RecommendationPercentage.Should().Be(67); // 2 out of 3
    }

    [Fact]
    public async Task CreateCompanyReview_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateCompanyReviewDto
        {
            CompanyID = 1,
            StudentID = 1,
            Rating = 4m,
            WouldRecommend = true,
            IsAnonymous = false
        };

        // Act - No authorization header
        var response = await _client.PostAsJsonAsync("/api/reviews/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        await dbContext.SaveChangesAsync();

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

    private async Task<CompletedOpportunity> SeedCompletedOpportunityAsync(int studentId, int companyId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        // Get the company's UserID for creating the project
        var company = await dbContext.Companies.FindAsync(companyId);
        if (company == null) throw new InvalidOperationException($"Company {companyId} not found");
        
        // Create a project first (required FK for CompletedOpportunity)
        var project = TestDataGenerator.CreateTestProject(companyId, company.UserID);
        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();
        
        var opportunity = TestDataGenerator.CreateTestCompletedOpportunity(studentId, companyId, project.ProjectID);
        await dbContext.CompletedOpportunities.AddAsync(opportunity);
        await dbContext.SaveChangesAsync();
        return opportunity;
    }

    private async Task<CompanyReview> SeedCompanyReviewAsync(
        int companyId, 
        int studentId, 
        ReviewStatus status = ReviewStatus.Approved,
        decimal rating = 4.5m,
        bool wouldRecommend = true)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var review = TestDataGenerator.CreateTestCompanyReview(companyId, studentId);
        review.Status = status;
        review.Rating = rating;
        review.WouldRecommend = wouldRecommend;
        
        await dbContext.CompanyReviews.AddAsync(review);
        await dbContext.SaveChangesAsync();
        return review;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
