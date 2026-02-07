using Sh8lny.Shared.DTOs.Chat;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Abstraction.Services;

/// <summary>
/// Interface for chat operations between users.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends a message to another user.
    /// Creates a new conversation if one doesn't exist.
    /// </summary>
    /// <param name="senderId">The sender's user ID.</param>
    /// <param name="dto">The message data.</param>
    /// <returns>Service response containing the sent message.</returns>
    Task<ServiceResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto);

    /// <summary>
    /// Gets all conversations for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Service response containing the list of conversations.</returns>
    Task<ServiceResponse<IEnumerable<ConversationDto>>> GetMyConversationsAsync(int userId);

    /// <summary>
    /// Gets all messages in a specific conversation.
    /// </summary>
    /// <param name="userId">The user ID (must be a participant).</param>
    /// <param name="conversationId">The conversation ID.</param>
    /// <returns>Service response containing the list of messages.</returns>
    Task<ServiceResponse<IEnumerable<MessageDto>>> GetConversationMessagesAsync(int userId, int conversationId);

    /// <summary>
    /// Marks all messages in a conversation as read for the user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="conversationId">The conversation ID.</param>
    /// <returns>Service response indicating success or failure.</returns>
    Task<ServiceResponse<bool>> MarkConversationAsReadAsync(int userId, int conversationId);
}
