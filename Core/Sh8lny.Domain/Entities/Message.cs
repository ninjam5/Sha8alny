namespace Sh8lny.Domain.Entities;

/// <summary>
/// Message entity for chat system
/// </summary>
public class Message
{
    // Primary key
    public int MessageID { get; set; }

    // Foreign keys
    public int ConversationID { get; set; }
    public int SenderID { get; set; }

    // Message content
    public required string MessageText { get; set; }
    public MessageType MessageType { get; set; }

    // Attachments
    public string? AttachmentURL { get; set; }
    public string? AttachmentName { get; set; }

    // Message status
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }

    // Timestamps
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }

    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
}

/// <summary>
/// Message type enumeration
/// </summary>
public enum MessageType
{
    Text,
    File,
    Image,
    Link
}
