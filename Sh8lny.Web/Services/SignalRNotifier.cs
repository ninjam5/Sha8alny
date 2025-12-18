using Microsoft.AspNetCore.SignalR;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Notifications;
using Sh8lny.Web.Hubs;

namespace Sh8lny.Web.Services;

/// <summary>
/// SignalR implementation of INotifier for real-time notification delivery.
/// </summary>
public class SignalRNotifier : INotifier
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotifier> _logger;

    public SignalRNotifier(IHubContext<NotificationHub> hubContext, ILogger<SignalRNotifier> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendNotificationAsync(int userId, NotificationDto notification)
    {
        try
        {
            // SignalR uses UserIdentifier which is the ClaimTypes.NameIdentifier from JWT
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notification);

            _logger.LogInformation("Real-time notification sent to user {UserId}: {Title}", 
                userId, notification.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send real-time notification to user {UserId}", userId);
            // Don't throw - real-time delivery failure shouldn't break the main operation
        }
    }

    /// <inheritdoc />
    public async Task SendNotificationToManyAsync(IEnumerable<int> userIds, NotificationDto notification)
    {
        try
        {
            var userIdStrings = userIds.Select(id => id.ToString()).ToList();
            
            await _hubContext.Clients.Users(userIdStrings)
                .SendAsync("ReceiveNotification", notification);

            _logger.LogInformation("Real-time notification sent to {Count} users: {Title}", 
                userIdStrings.Count, notification.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send real-time notification to multiple users");
            // Don't throw - real-time delivery failure shouldn't break the main operation
        }
    }
}
