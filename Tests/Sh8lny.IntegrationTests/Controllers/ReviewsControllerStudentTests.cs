using FluentAssertions;
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
/// Integration tests for ReviewsController - Student Reviews
/// </summary>
public class ReviewsControllerStudentTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReviewsControllerStudentTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateStudentReview_WithValidData_ReturnsCreatedReview()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateStudentReviewDto
        {
            StudentID = student.StudentID,
            CompanyID = company.CompanyID,
            Rating = 4.7m,
            ReviewTitle = "Outstanding Performance",
            ReviewText = "This student demonstrated exceptional skills.",
            TechnicalSkillsRating = 4.8m,
            CommunicationRating = 4.5m,
            ProfessionalismRating = 5m,
            TimeManagementRating = 4.5m,
            TeamworkRating = 4.7m,
            ProblemSolvingRating = 4.6m,
            WouldHireAgain = true,
            Strengths = "Great problem solver, quick learner",
            AreasForImprovement = "Time estimation",
            IsPublic = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/students", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var review = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        review.Should().NotBeNull();
        review!.ReviewID.Should().BeGreaterThan(0);
        review.Rating.Should().Be(4.7m);
        review.Status.Should().Be("Pending");
        review.IsPublic.Should().BeTrue();
        review.WouldHireAgain.Should().BeTrue();
    }

    [Fact]
    public async Task CreateStudentReview_AsPrivate_CreatesNonPublicReview()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateStudentReviewDto
        {
            StudentID = student.StudentID,
            CompanyID = company.CompanyID,
            Rating = 3m,
            ReviewText = "Needs improvement",
            WouldHireAgain = false,
            IsPublic = false // Private review
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/students", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var review = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        review.Should().NotBeNull();
        review!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public async Task GetStudentReviews_ReturnsOnlyPublicApprovedReviews()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create reviews with different visibility and status
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: true);
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: true);
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: false); // Not public
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Pending, isPublic: true); // Not approved

        // Act
        var response = await _client.GetAsync($"/api/reviews/students/by-student/{student.StudentID}?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<StudentReviewDto>>();
        reviews.Should().NotBeNull();
        reviews!.Count.Should().Be(2); // Only public AND approved
        reviews.All(r => r.Status == "Approved" && r.IsPublic).Should().BeTrue();
    }

    [Fact]
    public async Task GetCompanyStudentReviews_ReturnsAllReviewsByCompany()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var (student2User, student2, _, _) = await SeedTestDataAsync();
        
        // Company reviews multiple students
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID);
        await SeedStudentReviewAsync(student2.StudentID, company.CompanyID);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/reviews/students/by-company/{company.CompanyID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<StudentReviewDto>>();
        reviews.Should().NotBeNull();
        reviews!.Count.Should().Be(2);
        reviews.All(r => r.CompanyID == company.CompanyID).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStudentReview_AsOwner_UpdatesSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateStudentReviewDto
        {
            ReviewID = review.ReviewID,
            Rating = 5m,
            ReviewTitle = "Updated - Exceptional Talent",
            WouldHireAgain = true,
            IsPublic = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/students/{review.ReviewID}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        updated.Should().NotBeNull();
        updated!.Rating.Should().Be(5m);
        updated.ReviewTitle.Should().Be("Updated - Exceptional Talent");
        updated.Status.Should().Be("Pending"); // Reset after update
        
        // Verify in database using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var dbReview = await db.StudentReviews.FindAsync(review.ReviewID);
        dbReview.Should().NotBeNull();
        dbReview!.Rating.Should().Be(5m);
    }

    [Fact]
    public async Task UpdateStudentReview_ChangeFromPrivateToPublic_UpdatesVisibility()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, isPublic: false);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateDto = new UpdateStudentReviewDto
        {
            ReviewID = review.ReviewID,
            IsPublic = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/reviews/students/{review.ReviewID}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        updated.Should().NotBeNull();
        updated!.IsPublic.Should().BeTrue();
        
        // Verify in database using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var dbReview = await db.StudentReviews.FindAsync(review.ReviewID);
        dbReview.Should().NotBeNull();
        dbReview!.IsPublic.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStudentReview_AsOwner_DeletesSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID);

        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/reviews/students/{review.ReviewID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify deletion using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var deletedReview = await db.StudentReviews.FindAsync(review.ReviewID);
        deletedReview.Should().BeNull();
    }

    [Fact]
    public async Task AddStudentResponse_AsStudent_AddsResponseSuccessfully()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var review = await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved);

        var token = JwtTokenHelper.GenerateStudentToken(studentUser.UserID, studentUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseDto = new StudentResponseDto
        {
            ReviewID = review.ReviewID,
            StudentResponse = "Thank you for the valuable feedback!"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/reviews/students/{review.ReviewID}/response", responseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        updated.Should().NotBeNull();
        updated!.StudentResponse.Should().Be("Thank you for the valuable feedback!");
        updated.StudentRespondedAt.Should().NotBeNull();
        
        // Verify in database using fresh scope
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var dbReview = await db.StudentReviews.FindAsync(review.ReviewID);
        dbReview.Should().NotBeNull();
        dbReview!.StudentResponse.Should().Be("Thank you for the valuable feedback!");
    }

    [Fact]
    public async Task GetStudentReviewStats_ReturnsAccurateStatistics()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        
        // Create public approved reviews with known ratings
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: true, rating: 5m, wouldHireAgain: true);
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: true, rating: 4.5m, wouldHireAgain: true);
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: true, rating: 4m, wouldHireAgain: false);
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Pending, isPublic: true, rating: 5m); // Not counted
        await SeedStudentReviewAsync(student.StudentID, company.CompanyID, ReviewStatus.Approved, isPublic: false, rating: 5m); // Not counted

        // Act
        var response = await _client.GetAsync($"/api/reviews/students/stats/{student.StudentID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<ReviewStatsDto>();
        stats.Should().NotBeNull();
        stats!.TotalReviews.Should().Be(3); // Only public and approved
        stats.AverageRating.Should().BeApproximately(4.5m, 0.01m);
        stats.FiveStarCount.Should().Be(2); // 5 and 4.5
        stats.FourStarCount.Should().Be(1); // 4
        stats.RecommendationPercentage.Should().Be(67); // 2 "would hire again" out of 3
    }

    [Fact]
    public async Task CreateStudentReview_WithAllDetailedRatings_SavesAllFields()
    {
        // Arrange
        var (studentUser, student, companyUser, company) = await SeedTestDataAsync();
        var token = JwtTokenHelper.GenerateCompanyToken(companyUser.UserID, companyUser.Email);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateStudentReviewDto
        {
            StudentID = student.StudentID,
            CompanyID = company.CompanyID,
            Rating = 4.8m,
            TechnicalSkillsRating = 5m,
            CommunicationRating = 4.7m,
            ProfessionalismRating = 4.9m,
            TimeManagementRating = 4.6m,
            TeamworkRating = 4.8m,
            ProblemSolvingRating = 5m,
            WouldHireAgain = true,
            IsPublic = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reviews/students", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var review = await response.Content.ReadFromJsonAsync<StudentReviewDto>();
        review.Should().NotBeNull();
        review!.TechnicalSkillsRating.Should().Be(5m);
        review.CommunicationRating.Should().Be(4.7m);
        review.ProfessionalismRating.Should().Be(4.9m);
        review.TimeManagementRating.Should().Be(4.6m);
        review.TeamworkRating.Should().Be(4.8m);
        review.ProblemSolvingRating.Should().Be(5m);
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

    private async Task<StudentReview> SeedStudentReviewAsync(
        int studentId,
        int companyId,
        ReviewStatus status = ReviewStatus.Approved,
        bool isPublic = true,
        decimal rating = 4.5m,
        bool wouldHireAgain = true)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        
        var review = TestDataGenerator.CreateTestStudentReview(studentId, companyId);
        review.Status = status;
        review.IsPublic = isPublic;
        review.Rating = rating;
        review.WouldHireAgain = wouldHireAgain;
        
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
