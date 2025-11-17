using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sh8lny.Application.DTOs.Notifications;
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
/// Integration tests for NotificationsController
/// </summary>
public class NotificationsControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public NotificationsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Note: Database schema is created once by IAsyncLifetime.InitializeAsync()
        // Each test seeds its own data inline, so no need to clear or recreate schema here
    }

    [Fact]
    public async Task GetUserNotifications_ReturnsUserNotificationsPaginated()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync();
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        result.Should().NotBeNull();
        result!.Count.Should().Be(notifications.Count);
    }

    [Fact]
    public async Task GetUnreadCount_ReturnsCorrectCount()
    {
        // Arrange - Create 3 unread and 2 read notifications
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 3, readCount: 2);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications/unread-count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        result.Should().NotBeNull();
        result!["unreadCount"].Should().Be(3);
    }

    [Fact]
    public async Task MarkAsRead_WithOwnNotification_ReturnsOk()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 2);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var notificationId = notifications[0].NotificationID;

        // Act
        var response = await _client.PutAsync($"/api/notifications/{notificationId}/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify it was actually marked as read
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var notification = await db.Notifications.FindAsync(notificationId);
        notification.Should().NotBeNull();
        notification!.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task MarkAsRead_WithAnotherUserNotification_ReturnsForbidden()
    {
        // Arrange
        var (user1, notifications1) = await SeedUserWithNotificationsAsync(unreadCount: 1);
        var (user2, notifications2) = await SeedUserWithNotificationsAsync(unreadCount: 1);

        // User2 tries to mark User1's notification as read
        var token = JwtTokenHelper.GenerateJwtToken(user2.UserID, user2.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var user1NotificationId = notifications1[0].NotificationID;

        // Act
        var response = await _client.PutAsync($"/api/notifications/{user1NotificationId}/read", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task MarkAllAsRead_MarksAllUserNotificationsAsRead()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 5);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsync("/api/notifications/read-all", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify all notifications are marked as read
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var userNotifications = db.Notifications.Where(n => n.UserID == user.UserID).ToList();
        userNotifications.All(n => n.IsRead).Should().BeTrue();
        userNotifications.All(n => n.ReadAt != null).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteNotification_WithOwnNotification_ReturnsNoContent()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 2);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var notificationId = notifications[0].NotificationID;

        // Act
        var response = await _client.DeleteAsync($"/api/notifications/{notificationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it was actually deleted
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var notification = await db.Notifications.FindAsync(notificationId);
        notification.Should().BeNull();
    }

    [Fact]
    public async Task DeleteNotification_WithAnotherUserNotification_ReturnsForbidden()
    {
        // Arrange
        var (user1, notifications1) = await SeedUserWithNotificationsAsync(unreadCount: 1);
        var (user2, notifications2) = await SeedUserWithNotificationsAsync(unreadCount: 1);

        // User2 tries to delete User1's notification
        var token = JwtTokenHelper.GenerateJwtToken(user2.UserID, user2.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var user1NotificationId = notifications1[0].NotificationID;

        // Act
        var response = await _client.DeleteAsync($"/api/notifications/{user1NotificationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Verify notification still exists
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();
        var notification = await db.Notifications.FindAsync(user1NotificationId);
        notification.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserNotifications_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act - No authorization header
        var response = await _client.GetAsync("/api/notifications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsOnlyUnreadNotifications()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 3, readCount: 2);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications/unread");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        result.Should().NotBeNull();
        result!.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetNotificationById_WithOwnNotification_ReturnsNotification()
    {
        // Arrange
        var (user, notifications) = await SeedUserWithNotificationsAsync(unreadCount: 1);
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var notificationId = notifications[0].NotificationID;

        // Act
        var response = await _client.GetAsync($"/api/notifications/{notificationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<NotificationDto>();
        result.Should().NotBeNull();
        result!.NotificationID.Should().Be(notificationId);
        result.UserID.Should().Be(user.UserID);
    }

    [Fact]
    public async Task GetNotificationById_WithAnotherUserNotification_ReturnsForbidden()
    {
        // Arrange
        var (user1, notifications1) = await SeedUserWithNotificationsAsync(unreadCount: 1);
        var (user2, notifications2) = await SeedUserWithNotificationsAsync(unreadCount: 1);

        // User2 tries to get User1's notification
        var token = JwtTokenHelper.GenerateJwtToken(user2.UserID, user2.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var user1NotificationId = notifications1[0].NotificationID;

        // Act
        var response = await _client.GetAsync($"/api/notifications/{user1NotificationId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetNotificationStats_ReturnsAccurateStatistics()
    {
        // Arrange - Create notifications of different types
        var (user, _) = await SeedUserWithNotificationsAsync(
            unreadCount: 3,
            readCount: 2,
            useMultipleTypes: true
        );
        var token = JwtTokenHelper.GenerateJwtToken(user.UserID, user.Email, "Student");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/notifications/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<NotificationStatsDto>();
        result.Should().NotBeNull();
        result!.TotalNotifications.Should().Be(5);
        result.UnreadCount.Should().Be(3);
    }

    #region Helper Methods

    /// <summary>
    /// Seeds a user with notifications for testing
    /// </summary>
    private async Task<(User user, List<Notification> notifications)> SeedUserWithNotificationsAsync(
        int unreadCount = 3,
        int readCount = 0,
        bool useMultipleTypes = false)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Sha8lnyDbContext>();

        // Create user
        var user = TestDataGenerator.CreateTestUser(UserType.Student);
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var notifications = new List<Notification>();

        // Create unread notifications
        for (int i = 0; i < unreadCount; i++)
        {
            var notification = new Notification
            {
                UserID = user.UserID,
                NotificationType = useMultipleTypes 
                    ? (NotificationType)(i % 7) 
                    : NotificationType.System,
                Title = $"Test Notification {i + 1}",
                Message = $"This is test notification message {i + 1}",
                ActionURL = $"/test/{i + 1}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            };
            await dbContext.Notifications.AddAsync(notification);
            notifications.Add(notification);
        }

        // Create read notifications
        for (int i = 0; i < readCount; i++)
        {
            var notification = new Notification
            {
                UserID = user.UserID,
                NotificationType = useMultipleTypes 
                    ? (NotificationType)((i + unreadCount) % 7) 
                    : NotificationType.Message,
                Title = $"Read Notification {i + 1}",
                Message = $"This is a read notification message {i + 1}",
                ActionURL = $"/test/read/{i + 1}",
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddMinutes(-10 - i),
                CreatedAt = DateTime.UtcNow.AddMinutes(-20 - i)
            };
            await dbContext.Notifications.AddAsync(notification);
            notifications.Add(notification);
        }

        await dbContext.SaveChangesAsync();

        // Clear change tracker to force controller's scope to query fresh
        dbContext.ChangeTracker.Clear();

        return (user, notifications);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #endregion
}
