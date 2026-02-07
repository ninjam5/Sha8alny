namespace Sh8lny.Shared.DTOs.Chat;

/// <summary>
/// DTO representing a chat message.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Message ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Conversation ID this message belongs to.
    /// </summary>
    public int ConversationId { get; set; }

    /// <summary>
    /// The sender's user ID.
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// The sender's display name.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// The message content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Whether the message has been read by the recipient.
    /// </summary>
    public bool IsRead { get; set; }
}
