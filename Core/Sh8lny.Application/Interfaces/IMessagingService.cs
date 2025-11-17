using Sh8lny.Application.DTOs.Messaging;

namespace Sh8lny.Application.Interfaces;

/// <summary>
/// Service interface for messaging functionality
/// </summary>
public interface IMessagingService
{
    // Conversation operations
    Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto);
    Task<ConversationDto> GetConversationByIdAsync(int conversationId, int userId);
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId);
    Task<ConversationDto> UpdateConversationAsync(UpdateConversationDto dto, int userId);
    Task<bool> DeleteConversationAsync(int conversationId, int userId);

    // Participant operations
    Task<bool> AddParticipantsAsync(AddParticipantsDto dto, int requestingUserId);
    Task<bool> RemoveParticipantAsync(RemoveParticipantDto dto, int requestingUserId);
    Task<bool> LeaveConversationAsync(int conversationId, int userId);

    // Message operations
    Task<MessageDto> SendMessageAsync(SendMessageDto dto, int senderId);
    Task<MessageDto> GetMessageByIdAsync(int messageId, int userId);
    Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50);
    Task<MessageDto> EditMessageAsync(EditMessageDto dto, int userId);
    Task<bool> DeleteMessageAsync(int messageId, int userId);
    Task<bool> MarkMessagesAsReadAsync(MarkAsReadDto dto, int userId);

    // Unread count
    Task<int> GetUnreadCountAsync(int conversationId, int userId);
}
