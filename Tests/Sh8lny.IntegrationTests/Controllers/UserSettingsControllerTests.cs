using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.UserSettings;
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
/// Integration tests for UserSettingsController
/// </summary>
public class UserSettingsControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserSettingsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Note: Database schema is created once by IAsyncLifetime.InitializeAsync()
        // Each test seeds its own data inline, so no need to clear or recreate schema here
    }

    [Fact]
    public async Task GetMySettings_WhenUserHasNoSettings_ReturnsDefaultsAndCreatesRecord()
    {
        // Arrange - Seed only a user, no settings
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/usersettings/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.UserID.Should().Be(user.UserID);
        
        // Check default values
        result.EmailNotifications.Should().BeTrue();
        result.PushNotifications.Should().BeTrue();
        result.MessageNotifications.Should().BeTrue();
        result.ApplicationNotifications.Should().BeTrue();
        result.Language.Should().Be("en");
        result.Timezone.Should().Be("UTC");
        result.ProfileVisibility.Should().Be("Public");

        // Verify a UserSettings record was created in the database
        using (var verifyScope = _factory.Services.CreateScope())
        {
            var db = verifyScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            var settings = await db.UserSettings.FirstOrDefaultAsync(us => us.UserID == user.UserID);
            
            settings.Should().NotBeNull();
            settings!.UserID.Should().Be(user.UserID);
            settings.EmailNotifications.Should().BeTrue();
            settings.ProfileVisibility.Should().Be(ProfileVisibility.Public);
        }
    }

    [Fact]
    public async Task GetMySettings_WhenUserHasSettings_ReturnsExistingSettings()
    {
        // Arrange - Seed user with custom settings
        User user;
        Domain.Entities.UserSettings customSettings;
        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            // Create custom settings with non-default values
            customSettings = new Domain.Entities.UserSettings
            {
                UserID = user.UserID,
                EmailNotifications = false,
                PushNotifications = false,
                MessageNotifications = true,
                ApplicationNotifications = false,
                Language = "ar",
                Timezone = "Africa/Cairo",
                ProfileVisibility = ProfileVisibility.Private,
                UpdatedAt = DateTime.UtcNow
            };
            
            await db.UserSettings.AddAsync(customSettings);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/usersettings/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.UserID.Should().Be(user.UserID);
        
        // Verify non-default values are returned
        result.EmailNotifications.Should().BeFalse();
        result.PushNotifications.Should().BeFalse();
        result.MessageNotifications.Should().BeTrue();
        result.ApplicationNotifications.Should().BeFalse();
        result.Language.Should().Be("ar");
        result.Timezone.Should().Be("Africa/Cairo");
        result.ProfileVisibility.Should().Be("Private");
    }

    [Fact]
    public async Task UpdateMySettings_UpdatesSettingsSuccessfully()
    {
        // Arrange - Seed user (GetMySettings will create default settings on first call)
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create update DTO with new values
        var updateDto = new UpdateUserSettingsDto
        {
            UserID = user.UserID, // Will be overridden by controller
            EmailNotifications = false,
            PushNotifications = false,
            MessageNotifications = true,
            ApplicationNotifications = false,
            Language = "fr",
            Timezone = "Europe/Paris",
            ProfileVisibility = 1 // Private
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usersettings/me", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.UserID.Should().Be(user.UserID);
        result.EmailNotifications.Should().BeFalse();
        result.PushNotifications.Should().BeFalse();
        result.MessageNotifications.Should().BeTrue();
        result.ApplicationNotifications.Should().BeFalse();
        result.Language.Should().Be("fr");
        result.Timezone.Should().Be("Europe/Paris");
        result.ProfileVisibility.Should().Be("Private");

        // Verify changes were persisted to database
        using (var verifyScope = _factory.Services.CreateScope())
        {
            var db = verifyScope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            var settings = await db.UserSettings.FirstOrDefaultAsync(us => us.UserID == user.UserID);
            
            settings.Should().NotBeNull();
            settings!.EmailNotifications.Should().BeFalse();
            settings.PushNotifications.Should().BeFalse();
            settings.MessageNotifications.Should().BeTrue();
            settings.ApplicationNotifications.Should().BeFalse();
            settings.Language.Should().Be("fr");
            settings.Timezone.Should().Be("Europe/Paris");
            settings.ProfileVisibility.Should().Be(ProfileVisibility.Private);
        }
    }

    [Fact]
    public async Task GetMySettings_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act - No authorization header
        var response = await _client.GetAsync("/api/usersettings/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMySettings_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var updateDto = new UpdateUserSettingsDto
        {
            UserID = 999,
            EmailNotifications = false
        };

        // Act - No authorization header
        var response = await _client.PutAsJsonAsync("/api/usersettings/me", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateNotificationPreferences_UpdatesOnlyNotifications()
    {
        // Arrange - Seed user with settings
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var settings = new Domain.Entities.UserSettings
            {
                UserID = user.UserID,
                EmailNotifications = true,
                PushNotifications = true,
                MessageNotifications = true,
                ApplicationNotifications = true,
                Language = "en",
                Timezone = "UTC",
                ProfileVisibility = ProfileVisibility.Public,
                UpdatedAt = DateTime.UtcNow
            };
            
            await db.UserSettings.AddAsync(settings);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var notificationDto = new NotificationPreferencesDto
        {
            UserID = user.UserID,
            EmailNotifications = false,
            PushNotifications = false,
            MessageNotifications = false,
            ApplicationNotifications = false
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usersettings/notifications", notificationDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.EmailNotifications.Should().BeFalse();
        result.PushNotifications.Should().BeFalse();
        result.MessageNotifications.Should().BeFalse();
        result.ApplicationNotifications.Should().BeFalse();
        
        // Language and timezone should remain unchanged
        result.Language.Should().Be("en");
        result.Timezone.Should().Be("UTC");
        result.ProfileVisibility.Should().Be("Public");
    }

    [Fact]
    public async Task UpdatePrivacySettings_UpdatesOnlyPrivacy()
    {
        // Arrange - Seed user with settings
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var settings = new Domain.Entities.UserSettings
            {
                UserID = user.UserID,
                EmailNotifications = true,
                PushNotifications = true,
                MessageNotifications = true,
                ApplicationNotifications = true,
                Language = "en",
                Timezone = "UTC",
                ProfileVisibility = ProfileVisibility.Public,
                UpdatedAt = DateTime.UtcNow
            };
            
            await db.UserSettings.AddAsync(settings);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var privacyDto = new PrivacySettingsDto
        {
            UserID = user.UserID,
            ProfileVisibility = 2 // UniversityOnly
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usersettings/privacy", privacyDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.ProfileVisibility.Should().Be("UniversityOnly");
        
        // Notifications should remain unchanged
        result.EmailNotifications.Should().BeTrue();
        result.PushNotifications.Should().BeTrue();
        result.MessageNotifications.Should().BeTrue();
        result.ApplicationNotifications.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserSettings_ByUserId_ReturnsSettings()
    {
        // Arrange - Seed user with settings
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var settings = new Domain.Entities.UserSettings
            {
                UserID = user.UserID,
                EmailNotifications = true,
                PushNotifications = false,
                MessageNotifications = true,
                ApplicationNotifications = false,
                Language = "es",
                Timezone = "Europe/Madrid",
                ProfileVisibility = ProfileVisibility.UniversityOnly,
                UpdatedAt = DateTime.UtcNow
            };
            
            await db.UserSettings.AddAsync(settings);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/usersettings/{user.UserID}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.UserID.Should().Be(user.UserID);
        result.Language.Should().Be("es");
        result.Timezone.Should().Be("Europe/Madrid");
        result.ProfileVisibility.Should().Be("UniversityOnly");
    }

    [Fact]
    public async Task CreateDefaultSettings_CreatesSettingsSuccessfully()
    {
        // Arrange - Seed only a user
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync($"/api/usersettings/{user.UserID}/default", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.UserID.Should().Be(user.UserID);
        result.EmailNotifications.Should().BeTrue();
        result.ProfileVisibility.Should().Be("Public");
    }

    [Fact]
    public async Task UpdateMySettings_WithPartialData_UpdatesOnlyProvidedFields()
    {
        // Arrange - Seed user with existing settings
        User user;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
            
            user = TestDataGenerator.CreateTestUser(UserType.Student);
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            var settings = new Domain.Entities.UserSettings
            {
                UserID = user.UserID,
                EmailNotifications = true,
                PushNotifications = true,
                MessageNotifications = true,
                ApplicationNotifications = true,
                Language = "en",
                Timezone = "UTC",
                ProfileVisibility = ProfileVisibility.Public,
                UpdatedAt = DateTime.UtcNow
            };
            
            await db.UserSettings.AddAsync(settings);
            await db.SaveChangesAsync();
            
            db.ChangeTracker.Clear();
        }

        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Update only notification preferences, leave language/timezone/privacy unchanged
        var updateDto = new UpdateUserSettingsDto
        {
            UserID = user.UserID,
            EmailNotifications = false,
            // Other fields are null, should not be updated
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/usersettings/me", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        
        result.Should().NotBeNull();
        result!.EmailNotifications.Should().BeFalse(); // Changed
        
        // These should remain unchanged
        result.PushNotifications.Should().BeTrue();
        result.MessageNotifications.Should().BeTrue();
        result.ApplicationNotifications.Should().BeTrue();
        result.Language.Should().Be("en");
        result.Timezone.Should().Be("UTC");
        result.ProfileVisibility.Should().Be("Public");
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
