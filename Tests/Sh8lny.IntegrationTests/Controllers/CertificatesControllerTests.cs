using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.Certificates;
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
/// Integration tests for CertificatesController
/// </summary>
public class CertificatesControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CertificatesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Note: Database schema is created once by IAsyncLifetime.InitializeAsync()
        // Each test seeds its own data inline, so no need to clear or recreate schema here
    }

    [Fact]
    public async Task IssueCertificate_AsIssuingCompany_ReturnsCreated()
    {
        // Arrange - Seed company, student, and project
        var (company, student, project) = await SeedCompanyStudentAndProjectAsync();
        var companyToken = JwtTokenHelper.GenerateJwtToken(company.User.UserID, company.User.Email, "Company");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyToken);

        var dto = new IssueCertificateDto
        {
            StudentID = student.StudentID,
            ProjectID = project.ProjectID,
            CompanyID = company.CompanyID,
            CertificateNumber = Guid.NewGuid().ToString(),
            CertificateTitle = "Outstanding Performance Certificate",
            Description = "Awarded for exceptional work on the project",
            CertificateURL = "https://example.com/certificates/cert1.pdf",
            ExpiresAt = DateTime.UtcNow.AddYears(5)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/certificates", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CertificateDto>();
        result.Should().NotBeNull();
        result!.CertificateTitle.Should().Be("Outstanding Performance Certificate");
        result.StudentID.Should().Be(student.StudentID);
        result.ProjectID.Should().Be(project.ProjectID);
        result.CompanyID.Should().Be(company.CompanyID);

        // Verify certificate exists in database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var certificate = await db.Certificates.FindAsync(result.CertificateID);
        certificate.Should().NotBeNull();
        certificate!.CertificateTitle.Should().Be("Outstanding Performance Certificate");

        // Verify notification was created for the student
        var notification = db.Notifications
            .FirstOrDefault(n => n.UserID == student.UserID && n.NotificationType == NotificationType.Certificate);
        notification.Should().NotBeNull();
        notification!.Title.Should().Be("Certificate Issued");
    }

    [Fact]
    public async Task IssueCertificate_AsDifferentCompany_ReturnsForbidden()
    {
        // Arrange - Seed two companies, a student, and a project owned by CompanyA
        var (companyA, companyB, student, project) = await SeedTwoCompaniesStudentAndProjectAsync();
        
        // Authenticate as CompanyB
        var companyBToken = JwtTokenHelper.GenerateJwtToken(companyB.User.UserID, companyB.User.Email, "Company");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyBToken);

        var dto = new IssueCertificateDto
        {
            StudentID = student.StudentID,
            ProjectID = project.ProjectID, // This project belongs to CompanyA
            CompanyID = companyA.CompanyID,
            CertificateNumber = Guid.NewGuid().ToString(),
            CertificateTitle = "Unauthorized Certificate",
            Description = "This should fail"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/certificates", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task VerifyCertificate_WithValidNumber_ReturnsCertificateDetails()
    {
        // Arrange - Seed a full certificate
        var (certificate, company, student, project) = await SeedCertificateWithRelatedEntitiesAsync();

        var dto = new VerifyCertificateDto
        {
            CertificateNumber = certificate.CertificateNumber
        };

        // Act - No authentication required for verification
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/certificates/verify", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CertificateDto>();
        result.Should().NotBeNull();
        result!.CertificateID.Should().Be(certificate.CertificateID);
        result.CertificateNumber.Should().Be(certificate.CertificateNumber);
        result.StudentName.Should().Be(student.FullName);
        result.CompanyName.Should().Be(company.CompanyName);
        result.ProjectName.Should().Be(project.ProjectName);
    }

    [Fact]
    public async Task VerifyCertificate_WithInvalidNumber_ReturnsNotFound()
    {
        // Arrange
        var dto = new VerifyCertificateDto
        {
            CertificateNumber = "invalid-guid-token-12345"
        };

        // Act - No authentication required
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync("/api/certificates/verify", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStudentCertificates_ReturnsCertificates()
    {
        // Arrange - Seed a student with two certificates
        var (student, certificates) = await SeedStudentWithCertificatesAsync(certificateCount: 2);

        // Act - No authentication required for public endpoint
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/certificates/student/{student.StudentID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<CertificateDto>>();
        result.Should().NotBeNull();
        result!.Count.Should().Be(2);
        result.All(c => c.StudentID == student.StudentID).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteCertificate_AsIssuingCompany_ReturnsNoContent()
    {
        // Arrange - Seed a company and a certificate issued by them
        var (company, certificate) = await SeedCompanyWithCertificateAsync();
        var companyToken = JwtTokenHelper.GenerateJwtToken(company.User.UserID, company.User.Email, "Company");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyToken);

        // Act
        var response = await _client.DeleteAsync($"/api/certificates/{certificate.CertificateID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify certificate is deleted from database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var deletedCertificate = await db.Certificates.FindAsync(certificate.CertificateID);
        deletedCertificate.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCertificate_AsDifferentUser_ReturnsForbidden()
    {
        // Arrange - Seed a company, student, and certificate
        var (company, student, certificate) = await SeedCompanyStudentAndCertificateAsync();
        
        // Authenticate as the student (not the issuing company)
        var studentToken = JwtTokenHelper.GenerateJwtToken(student.User.UserID, student.User.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

        // Act
        var response = await _client.DeleteAsync($"/api/certificates/{certificate.CertificateID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCertificateById_ReturnsOk()
    {
        // Arrange
        var (certificate, _, _, _) = await SeedCertificateWithRelatedEntitiesAsync();

        // Act - No authentication required
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/certificates/{certificate.CertificateID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CertificateDto>();
        result.Should().NotBeNull();
        result!.CertificateID.Should().Be(certificate.CertificateID);
    }

    [Fact]
    public async Task GetCertificateByNumber_ReturnsOk()
    {
        // Arrange
        var (certificate, _, _, _) = await SeedCertificateWithRelatedEntitiesAsync();

        // Act - No authentication required
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/certificates/by-number/{certificate.CertificateNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CertificateDto>();
        result.Should().NotBeNull();
        result!.CertificateNumber.Should().Be(certificate.CertificateNumber);
    }

    #region Helper Methods

    /// <summary>
    /// Seeds a company, student, and project owned by the company
    /// </summary>
    private async Task<(Company company, Student student, Project project)> SeedCompanyStudentAndProjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create company user and profile
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = new Company
        {
            UserID = companyUser.UserID,
            CompanyName = "Tech Innovations Inc",
            ContactEmail = companyUser.Email,
            Industry = "Technology"
        };
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        // Create student user and profile
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "John",
            LastName = "Doe",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create project owned by company
        var project = new Project
        {
            CompanyID = company.CompanyID,
            ProjectName = "Web Development Project",
            Description = "A comprehensive web development project",
            Deadline = DateTime.UtcNow.AddMonths(3),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        // Reload with navigation properties
        company.User = companyUser;
        student.User = studentUser;

        return (company, student, project);
    }

    /// <summary>
    /// Seeds two companies, a student, and a project owned by the first company
    /// </summary>
    private async Task<(Company companyA, Company companyB, Student student, Project project)> SeedTwoCompaniesStudentAndProjectAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create CompanyA
        var companyUserA = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUserA);
        await db.SaveChangesAsync();

        var companyA = new Company
        {
            UserID = companyUserA.UserID,
            CompanyName = "Company A",
            ContactEmail = companyUserA.Email,
            Industry = "Technology"
        };
        await db.Companies.AddAsync(companyA);
        await db.SaveChangesAsync();

        // Create CompanyB
        var companyUserB = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUserB);
        await db.SaveChangesAsync();

        var companyB = new Company
        {
            UserID = companyUserB.UserID,
            CompanyName = "Company B",
            ContactEmail = companyUserB.Email,
            Industry = "Finance"
        };
        await db.Companies.AddAsync(companyB);
        await db.SaveChangesAsync();

        // Create student
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "Jane",
            LastName = "Smith",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create project owned by CompanyA
        var project = new Project
        {
            CompanyID = companyA.CompanyID,
            ProjectName = "CompanyA Project",
            Description = "A project owned by CompanyA",
            Deadline = DateTime.UtcNow.AddMonths(3),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        // Reload with navigation properties
        companyA.User = companyUserA;
        companyB.User = companyUserB;
        student.User = studentUser;

        return (companyA, companyB, student, project);
    }

    /// <summary>
    /// Seeds a complete certificate with all related entities
    /// </summary>
    private async Task<(Certificate certificate, Company company, Student student, Project project)> SeedCertificateWithRelatedEntitiesAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create company
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = new Company
        {
            UserID = companyUser.UserID,
            CompanyName = "Certificate Issuer Corp",
            ContactEmail = companyUser.Email,
            Industry = "Technology"
        };
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        // Create student
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "Alice",
            LastName = "Johnson",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create project
        var project = new Project
        {
            CompanyID = company.CompanyID,
            ProjectName = "Certification Project",
            Description = "Project for certificate testing",
            Deadline = DateTime.UtcNow.AddMonths(3),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        // Create certificate
        var certificate = new Certificate
        {
            StudentID = student.StudentID,
            ProjectID = project.ProjectID,
            CompanyID = company.CompanyID,
            CertificateNumber = Guid.NewGuid().ToString(),
            CertificateTitle = "Excellence in Development",
            Description = "Awarded for exceptional work",
            CertificateURL = "https://example.com/cert.pdf",
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(3)
        };
        await db.Certificates.AddAsync(certificate);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        return (certificate, company, student, project);
    }

    /// <summary>
    /// Seeds a student with multiple certificates
    /// </summary>
    private async Task<(Student student, List<Certificate> certificates)> SeedStudentWithCertificatesAsync(int certificateCount = 2)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create student
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "Bob",
            LastName = "Williams",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create company for certificates
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = new Company
        {
            UserID = companyUser.UserID,
            CompanyName = "Multiple Certs Company",
            ContactEmail = companyUser.Email,
            Industry = "Education"
        };
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        // Create project
        var project = new Project
        {
            CompanyID = company.CompanyID,
            ProjectName = "Training Program",
            Description = "Comprehensive training",
            Deadline = DateTime.UtcNow.AddMonths(6),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        // Create certificates
        var certificates = new List<Certificate>();
        for (int i = 0; i < certificateCount; i++)
        {
            var certificate = new Certificate
            {
                StudentID = student.StudentID,
                ProjectID = project.ProjectID,
                CompanyID = company.CompanyID,
                CertificateNumber = Guid.NewGuid().ToString(),
                CertificateTitle = $"Certificate {i + 1}",
                Description = $"Description for certificate {i + 1}",
                IssuedAt = DateTime.UtcNow.AddDays(-i),
                ExpiresAt = DateTime.UtcNow.AddYears(2)
            };
            await db.Certificates.AddAsync(certificate);
            certificates.Add(certificate);
        }

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        return (student, certificates);
    }

    /// <summary>
    /// Seeds a company with a certificate issued by them
    /// </summary>
    private async Task<(Company company, Certificate certificate)> SeedCompanyWithCertificateAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create company
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = new Company
        {
            UserID = companyUser.UserID,
            CompanyName = "Revoke Test Company",
            ContactEmail = companyUser.Email,
            Industry = "Technology"
        };
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        // Create student
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "Charlie",
            LastName = "Brown",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create project
        var project = new Project
        {
            CompanyID = company.CompanyID,
            ProjectName = "Revoke Test Project",
            Description = "Project for revoke testing",
            Deadline = DateTime.UtcNow.AddMonths(3),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        // Create certificate
        var certificate = new Certificate
        {
            StudentID = student.StudentID,
            ProjectID = project.ProjectID,
            CompanyID = company.CompanyID,
            CertificateNumber = Guid.NewGuid().ToString(),
            CertificateTitle = "Revokable Certificate",
            Description = "This certificate can be revoked",
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };
        await db.Certificates.AddAsync(certificate);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        // Reload with navigation properties
        company.User = companyUser;

        return (company, certificate);
    }

    /// <summary>
    /// Seeds a company, student, and certificate for authorization testing
    /// </summary>
    private async Task<(Company company, Student student, Certificate certificate)> SeedCompanyStudentAndCertificateAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create company
        var companyUser = TestDataGenerator.CreateTestUser(UserType.Company);
        await db.Users.AddAsync(companyUser);
        await db.SaveChangesAsync();

        var company = new Company
        {
            UserID = companyUser.UserID,
            CompanyName = "Auth Test Company",
            ContactEmail = companyUser.Email,
            Industry = "Technology"
        };
        await db.Companies.AddAsync(company);
        await db.SaveChangesAsync();

        // Create student
        var studentUser = TestDataGenerator.CreateTestUser(UserType.Student);
        await db.Users.AddAsync(studentUser);
        await db.SaveChangesAsync();

        var student = new Student
        {
            UserID = studentUser.UserID,
            FirstName = "David",
            LastName = "Miller",
            Country = "USA"
        };
        await db.Students.AddAsync(student);
        await db.SaveChangesAsync();

        // Create project
        var project = new Project
        {
            CompanyID = company.CompanyID,
            ProjectName = "Auth Test Project",
            Description = "Project for authorization testing",
            Deadline = DateTime.UtcNow.AddMonths(3),
            ProjectType = ProjectType.Internship
        };
        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        // Create certificate
        var certificate = new Certificate
        {
            StudentID = student.StudentID,
            ProjectID = project.ProjectID,
            CompanyID = company.CompanyID,
            CertificateNumber = Guid.NewGuid().ToString(),
            CertificateTitle = "Authorization Test Certificate",
            Description = "For testing authorization",
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };
        await db.Certificates.AddAsync(certificate);
        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        // Reload with navigation properties
        company.User = companyUser;
        student.User = studentUser;

        return (company, student, certificate);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
