using Sh8lny.Application.DTOs.Messaging;
using Sh8lny.Application.Interfaces;
using Sh8lny.Domain.Entities;
using Sh8lny.Domain.Exceptions;
using Sh8lny.Domain.Interfaces;

namespace Sh8lny.Application.UseCases.Messaging;

/// <summary>
/// Service for messaging functionality
/// </summary>
public class MessagingService : IMessagingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Conversation Operations

    public async Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto)
    {
        // Validate conversation type
        if (!Enum.IsDefined(typeof(ConversationType), dto.ConversationType))
            throw new ValidationException("Invalid conversation type");

        // Validate participants
        if (dto.ParticipantUserIDs == null || !dto.ParticipantUserIDs.Any())
            throw new ValidationException("At least one participant is required");

        // Create conversation entity
        var conversation = new Conversation
        {
            ConversationType = (ConversationType)dto.ConversationType,
            GroupID = dto.GroupID,
            ConversationName = dto.ConversationName,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Conversations.AddAsync(conversation);
        await _unitOfWork.SaveChangesAsync();

        // Add participants
        foreach (var userId in dto.ParticipantUserIDs)
        {
            var participant = new ConversationParticipant
            {
                ConversationID = conversation.ConversationID,
                UserID = userId,
                JoinedAt = DateTime.UtcNow
            };
            await _unitOfWork.ConversationParticipants.AddAsync(participant);
        }

        await _unitOfWork.SaveChangesAsync();

        // Return DTO
        return await GetConversationByIdAsync(conversation.ConversationID, dto.ParticipantUserIDs.First());
    }

    public async Task<ConversationDto> GetConversationByIdAsync(int conversationId, int userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), conversationId);

        // Check if user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(conversationId, userId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not a participant in this conversation");

        // Get participants
        var participants = await _unitOfWork.ConversationParticipants
            .GetByConversationIdAsync(conversationId);

        // Get last message
        var messages = await _unitOfWork.Messages.GetByConversationIdAsync(conversationId, 1, 1);
        var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

        // Get unread count
        var unreadCount = await GetUnreadCountAsync(conversationId, userId);

        return new ConversationDto
        {
            ConversationID = conversation.ConversationID,
            ConversationType = conversation.ConversationType.ToString(),
            GroupID = conversation.GroupID,
            ConversationName = conversation.ConversationName,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            Participants = participants.Select(p => new ParticipantDto
            {
                ParticipantID = p.ParticipantID,
                UserID = p.UserID,
                UserName = (p.User.FirstName ?? "") + " " + (p.User.LastName ?? ""),
                UserType = p.User.UserType.ToString(),
                JoinedAt = p.JoinedAt,
                LastReadAt = p.LastReadAt
            }).ToList(),
            LastMessage = lastMessage != null ? MapToMessageDto(lastMessage) : null,
            UnreadCount = unreadCount
        };
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId)
    {
        var participations = await _unitOfWork.ConversationParticipants.GetByUserIdAsync(userId);
        var conversations = new List<ConversationDto>();

        foreach (var participation in participations)
        {
            var conversationDto = await GetConversationByIdAsync(participation.ConversationID, userId);
            conversations.Add(conversationDto);
        }

        return conversations.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt);
    }

    public async Task<ConversationDto> UpdateConversationAsync(UpdateConversationDto dto, int userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationID);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), dto.ConversationID);

        // Check if user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(dto.ConversationID, userId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not authorized to update this conversation");

        // Update conversation name
        if (!string.IsNullOrWhiteSpace(dto.ConversationName))
        {
            conversation.ConversationName = dto.ConversationName;
            await _unitOfWork.Conversations.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();
        }

        return await GetConversationByIdAsync(dto.ConversationID, userId);
    }

    public async Task<bool> DeleteConversationAsync(int conversationId, int userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), conversationId);

        // Check if user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(conversationId, userId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not authorized to delete this conversation");

        // Delete all messages
        var messages = await _unitOfWork.Messages.GetByConversationIdAsync(conversationId);
        await _unitOfWork.Messages.DeleteRangeAsync(messages);

        // Delete all participants
        var participants = await _unitOfWork.ConversationParticipants.GetByConversationIdAsync(conversationId);
        await _unitOfWork.ConversationParticipants.DeleteRangeAsync(participants);

        // Delete conversation
        await _unitOfWork.Conversations.DeleteAsync(conversation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Participant Operations

    public async Task<bool> AddParticipantsAsync(AddParticipantsDto dto, int requestingUserId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationID);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), dto.ConversationID);

        // Check if requesting user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(dto.ConversationID, requestingUserId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not authorized to add participants");

        // Add new participants
        foreach (var userId in dto.UserIDs)
        {
            // Check if already participant
            var existingParticipant = await _unitOfWork.ConversationParticipants
                .IsUserParticipantAsync(dto.ConversationID, userId);
            
            if (existingParticipant)
                continue;

            var participant = new ConversationParticipant
            {
                ConversationID = dto.ConversationID,
                UserID = userId,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.ConversationParticipants.AddAsync(participant);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveParticipantAsync(RemoveParticipantDto dto, int requestingUserId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationID);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), dto.ConversationID);

        // Check if requesting user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(dto.ConversationID, requestingUserId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not authorized to remove participants");

        // Get participant to remove
        var participants = await _unitOfWork.ConversationParticipants.GetByConversationIdAsync(dto.ConversationID);
        var participantToRemove = participants.FirstOrDefault(p => p.UserID == dto.UserID);

        if (participantToRemove == null)
            throw new NotFoundException("Participant", dto.UserID);

        await _unitOfWork.ConversationParticipants.DeleteAsync(participantToRemove);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LeaveConversationAsync(int conversationId, int userId)
    {
        var participants = await _unitOfWork.ConversationParticipants.GetByConversationIdAsync(conversationId);
        var participant = participants.FirstOrDefault(p => p.UserID == userId);

        if (participant == null)
            throw new NotFoundException("Participant", userId);

        await _unitOfWork.ConversationParticipants.DeleteAsync(participant);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Message Operations

    public async Task<MessageDto> SendMessageAsync(SendMessageDto dto, int senderId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(dto.ConversationID);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), dto.ConversationID);

        // Check if sender is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(dto.ConversationID, senderId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not a participant in this conversation");

        // Validate message type
        if (!Enum.IsDefined(typeof(MessageType), dto.MessageType))
            throw new ValidationException("Invalid message type");

        // Create message
        var message = new Message
        {
            ConversationID = dto.ConversationID,
            SenderID = senderId,
            MessageText = dto.MessageText,
            MessageType = (MessageType)dto.MessageType,
            AttachmentURL = dto.AttachmentURL,
            AttachmentName = dto.AttachmentName,
            IsRead = false,
            IsEdited = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message);

        // Update conversation last message time
        conversation.LastMessageAt = DateTime.UtcNow;
        await _unitOfWork.Conversations.UpdateAsync(conversation);

        await _unitOfWork.SaveChangesAsync();

        return await GetMessageByIdAsync(message.MessageID, senderId);
    }

    public async Task<MessageDto> GetMessageByIdAsync(int messageId, int userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        if (message == null)
            throw new NotFoundException(nameof(Message), messageId);

        // Check if user is participant in conversation
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(message.ConversationID, userId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not authorized to view this message");

        return MapToMessageDto(message);
    }

    public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50)
    {
        // Check if user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .IsUserParticipantAsync(conversationId, userId);
        
        if (!isParticipant)
            throw new UnauthorizedException("You are not a participant in this conversation");

        var messages = await _unitOfWork.Messages.GetByConversationIdAsync(conversationId, page, pageSize);

        return messages.Select(MapToMessageDto);
    }

    public async Task<MessageDto> EditMessageAsync(EditMessageDto dto, int userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(dto.MessageID);
        if (message == null)
            throw new NotFoundException(nameof(Message), dto.MessageID);

        // Only sender can edit
        if (message.SenderID != userId)
            throw new UnauthorizedException("You can only edit your own messages");

        message.MessageText = dto.MessageText;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _unitOfWork.Messages.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return MapToMessageDto(message);
    }

    public async Task<bool> DeleteMessageAsync(int messageId, int userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        if (message == null)
            throw new NotFoundException(nameof(Message), messageId);

        // Only sender can delete
        if (message.SenderID != userId)
            throw new UnauthorizedException("You can only delete your own messages");

        await _unitOfWork.Messages.DeleteAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkMessagesAsReadAsync(MarkAsReadDto dto, int userId)
    {
        // Update participant's LastReadAt
        var participants = await _unitOfWork.ConversationParticipants.GetByConversationIdAsync(dto.ConversationID);
        var participant = participants.FirstOrDefault(p => p.UserID == userId);

        if (participant == null)
            throw new NotFoundException("Participant", userId);

        participant.LastReadAt = dto.LastReadAt ?? DateTime.UtcNow;
        await _unitOfWork.ConversationParticipants.UpdateAsync(participant);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetUnreadCountAsync(int conversationId, int userId)
    {
        var participants = await _unitOfWork.ConversationParticipants.GetByConversationIdAsync(conversationId);
        var participant = participants.FirstOrDefault(p => p.UserID == userId);

        if (participant == null)
            return 0;

        var messages = await _unitOfWork.Messages.GetByConversationIdAsync(conversationId);
        
        var unreadCount = messages.Count(m => 
            m.SenderID != userId && 
            (participant.LastReadAt == null || m.SentAt > participant.LastReadAt));

        return unreadCount;
    }

    #endregion

    #region Helper Methods

    private MessageDto MapToMessageDto(Message message)
    {
        return new MessageDto
        {
            MessageID = message.MessageID,
            ConversationID = message.ConversationID,
            SenderID = message.SenderID,
            SenderName = (message.Sender.FirstName ?? "") + " " + (message.Sender.LastName ?? ""),
            MessageText = message.MessageText,
            MessageType = message.MessageType.ToString(),
            AttachmentURL = message.AttachmentURL,
            AttachmentName = message.AttachmentName,
            IsRead = message.IsRead,
            IsEdited = message.IsEdited,
            SentAt = message.SentAt,
            EditedAt = message.EditedAt
        };
    }

    #endregion
}
