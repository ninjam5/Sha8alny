namespace Sh8lny.Shared.DTOs.Chat;

/// <summary>
/// DTO representing a conversation summary.
/// </summary>
public class ConversationDto
{
    /// <summary>
    /// Conversation ID.
    /// </summary>
    public int ConversationId { get; set; }

    /// <summary>
    /// The other participant's user ID.
    /// </summary>
    public int OtherUserId { get; set; }

    /// <summary>
    /// The other participant's display name.
    /// </summary>
    public string OtherUserName { get; set; } = string.Empty;

    /// <summary>
    /// The other participant's profile picture URL.
    /// </summary>
    public string? OtherUserProfilePicture { get; set; }

    /// <summary>
    /// The last message in the conversation.
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// When the last message was sent.
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Number of unread messages in this conversation.
    /// </summary>
    public int UnreadCount { get; set; }
}
