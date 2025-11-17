using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Application.DTOs.Messaging;

#region Conversation DTOs

/// <summary>
/// DTO for creating a new conversation (direct or group)
/// </summary>
public class CreateConversationDto
{
    [Required(ErrorMessage = "Conversation type is required")]
    public int ConversationType { get; set; } // 0 = Direct, 1 = Group

    [MaxLength(100, ErrorMessage = "Conversation name cannot exceed 100 characters")]
    public string? ConversationName { get; set; }

    public int? GroupID { get; set; }

    [Required(ErrorMessage = "At least one participant is required")]
    [MinLength(1, ErrorMessage = "At least one participant is required")]
    public List<int> ParticipantUserIDs { get; set; } = new();
}

/// <summary>
/// DTO for conversation details
/// </summary>
public class ConversationDto
{
    public int ConversationID { get; set; }
    public string ConversationType { get; set; } = string.Empty;
    public int? GroupID { get; set; }
    public string? ConversationName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}

/// <summary>
/// DTO for updating conversation settings
/// </summary>
public class UpdateConversationDto
{
    [Required(ErrorMessage = "Conversation ID is required")]
    public int ConversationID { get; set; }

    [MaxLength(100, ErrorMessage = "Conversation name cannot exceed 100 characters")]
    public string? ConversationName { get; set; }
}

/// <summary>
/// DTO for adding participants to a conversation
/// </summary>
public class AddParticipantsDto
{
    [Required(ErrorMessage = "Conversation ID is required")]
    public int ConversationID { get; set; }

    [Required(ErrorMessage = "At least one user ID is required")]
    [MinLength(1, ErrorMessage = "At least one user ID is required")]
    public List<int> UserIDs { get; set; } = new();
}

/// <summary>
/// DTO for removing a participant from a conversation
/// </summary>
public class RemoveParticipantDto
{
    [Required(ErrorMessage = "Conversation ID is required")]
    public int ConversationID { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int UserID { get; set; }
}

#endregion

#region Message DTOs

/// <summary>
/// DTO for sending a new message
/// </summary>
public class SendMessageDto
{
    [Required(ErrorMessage = "Conversation ID is required")]
    public int ConversationID { get; set; }

    [Required(ErrorMessage = "Message text is required")]
    [MaxLength(5000, ErrorMessage = "Message text cannot exceed 5000 characters")]
    public string MessageText { get; set; } = string.Empty;

    public int MessageType { get; set; } // 0 = Text, 1 = File, 2 = Image, 3 = Link

    [Url(ErrorMessage = "Invalid URL format")]
    public string? AttachmentURL { get; set; }

    [MaxLength(255, ErrorMessage = "Attachment name cannot exceed 255 characters")]
    public string? AttachmentName { get; set; }
}

/// <summary>
/// DTO for message details
/// </summary>
public class MessageDto
{
    public int MessageID { get; set; }
    public int ConversationID { get; set; }
    public int SenderID { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public string? AttachmentURL { get; set; }
    public string? AttachmentName { get; set; }
    public bool IsRead { get; set; }
    public bool IsEdited { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }
}

/// <summary>
/// DTO for editing a message
/// </summary>
public class EditMessageDto
{
    [Required(ErrorMessage = "Message ID is required")]
    public int MessageID { get; set; }

    [Required(ErrorMessage = "Message text is required")]
    [MaxLength(5000, ErrorMessage = "Message text cannot exceed 5000 characters")]
    public string MessageText { get; set; } = string.Empty;
}

/// <summary>
/// DTO for marking messages as read
/// </summary>
public class MarkAsReadDto
{
    [Required(ErrorMessage = "Conversation ID is required")]
    public int ConversationID { get; set; }

    public DateTime? LastReadAt { get; set; }
}

#endregion

#region Participant DTOs

/// <summary>
/// DTO for conversation participant details
/// </summary>
public class ParticipantDto
{
    public int ParticipantID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
}

#endregion
