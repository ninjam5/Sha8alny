using Bogus;
using Sh8lny.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Sh8lny.IntegrationTests.Helpers;

/// <summary>
/// Helper class to generate test data using Bogus
/// </summary>
public static class TestDataGenerator
{
    private static readonly Faker _faker = new Faker();

    /// <summary>
    /// Creates a test user entity
    /// </summary>
    public static User CreateTestUser(UserType userType = UserType.Student, string? email = null)
    {
        var user = new User
        {
            Email = email ?? _faker.Internet.Email(),
            PasswordHash = HashPassword("Test@123"),
            UserType = userType,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        return user;
    }

    /// <summary>
    /// Creates a test student entity
    /// </summary>
    public static Student CreateTestStudent(int userId)
    {
        return new Student
        {
            UserID = userId,
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Phone = _faker.Phone.PhoneNumber(),
            Country = _faker.Address.Country(),
            AcademicYear = AcademicYear.ThirdYear,
            Status = StudentStatus.Active,
            ProfileCompleteness = 80,
            AverageRating = 0,
            TotalReviews = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test company entity
    /// </summary>
    public static Company CreateTestCompany(int userId)
    {
        return new Company
        {
            UserID = userId,
            CompanyName = _faker.Company.CompanyName(),
            ContactEmail = _faker.Internet.Email(),
            ContactPhone = _faker.Phone.PhoneNumber(),
            Industry = _faker.Commerce.Department(),
            Description = _faker.Company.CatchPhrase(),
            Country = _faker.Address.Country(),
            City = _faker.Address.City(),
            AverageRating = 0,
            TotalReviews = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test completed opportunity entity
    /// </summary>
    public static CompletedOpportunity CreateTestCompletedOpportunity(int studentId, int companyId, int projectId)
    {
        return new CompletedOpportunity
        {
            StudentID = studentId,
            ProjectID = projectId,
            OpportunityTitle = "Test Project Opportunity",
            OpportunityType = OpportunityType.Internship,
            StartDate = DateTime.UtcNow.AddDays(-90),
            EndDate = DateTime.UtcNow.AddDays(-10),
            DurationInDays = 80,
            Status = CompletionStatus.Completed,
            IsVerified = true,
            VerifiedAt = DateTime.UtcNow.AddDays(-5)
        };
    }

    /// <summary>
    /// Creates a test project entity
    /// </summary>
    public static Project CreateTestProject(int companyId, int createdBy)
    {
        return new Project
        {
            CompanyID = companyId,
            ProjectName = _faker.Commerce.ProductName(),
            Description = _faker.Lorem.Paragraph(),
            ProjectType = ProjectType.Internship,
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(97),
            Deadline = DateTime.UtcNow.AddDays(5),
            Duration = "3 months",
            Status = ProjectStatus.Active,
            IsVisible = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test company review entity
    /// </summary>
    public static CompanyReview CreateTestCompanyReview(int companyId, int studentId, int? opportunityId = null)
    {
        return new CompanyReview
        {
            CompanyID = companyId,
            StudentID = studentId,
            CompletedOpportunityID = opportunityId,
            Rating = (decimal)_faker.Random.Double(3, 5),
            ReviewTitle = _faker.Lorem.Sentence(5),
            ReviewText = _faker.Lorem.Paragraph(),
            WorkEnvironmentRating = (decimal)_faker.Random.Double(3, 5),
            LearningOpportunityRating = (decimal)_faker.Random.Double(3, 5),
            MentorshipRating = (decimal)_faker.Random.Double(3, 5),
            CompensationRating = (decimal)_faker.Random.Double(3, 5),
            CommunicationRating = (decimal)_faker.Random.Double(3, 5),
            WouldRecommend = _faker.Random.Bool(0.8f),
            Pros = _faker.Lorem.Sentence(10),
            Cons = _faker.Lorem.Sentence(5),
            Status = ReviewStatus.Approved,
            IsVerified = opportunityId.HasValue,
            IsAnonymous = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test student review entity
    /// </summary>
    public static StudentReview CreateTestStudentReview(int studentId, int companyId, int? opportunityId = null)
    {
        return new StudentReview
        {
            StudentID = studentId,
            CompanyID = companyId,
            CompletedOpportunityID = opportunityId,
            Rating = (decimal)_faker.Random.Double(3, 5),
            ReviewTitle = _faker.Lorem.Sentence(5),
            ReviewText = _faker.Lorem.Paragraph(),
            TechnicalSkillsRating = (decimal)_faker.Random.Double(3, 5),
            CommunicationRating = (decimal)_faker.Random.Double(3, 5),
            ProfessionalismRating = (decimal)_faker.Random.Double(3, 5),
            TimeManagementRating = (decimal)_faker.Random.Double(3, 5),
            TeamworkRating = (decimal)_faker.Random.Double(3, 5),
            ProblemSolvingRating = (decimal)_faker.Random.Double(3, 5),
            WouldHireAgain = _faker.Random.Bool(0.8f),
            Strengths = _faker.Lorem.Sentence(10),
            AreasForImprovement = _faker.Lorem.Sentence(5),
            Status = ReviewStatus.Approved,
            IsVerified = opportunityId.HasValue,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Simple password hasher for testing
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
