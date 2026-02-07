using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Shared.DTOs.Chat;

/// <summary>
/// DTO for sending a message to another user.
/// </summary>
public class SendMessageDto
{
    /// <summary>
    /// The user ID of the message receiver.
    /// </summary>
    [Required]
    public int ReceiverId { get; set; }

    /// <summary>
    /// The message content.
    /// </summary>
    [Required]
    [StringLength(4000, ErrorMessage = "Message cannot exceed 4000 characters.")]
    public string Content { get; set; } = string.Empty;
}
