using Sh8lny.Abstraction.Repositories;
using Sh8lny.Abstraction.Services;
using Sh8lny.Domain.Models;
using Sh8lny.Shared.DTOs.Chat;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Service;

/// <summary>
/// Service for chat operations between users.
/// </summary>
public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public ChatService(IUnitOfWork unitOfWork, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto)
    {
        try
        {
            // 1. Verify sender exists
            var sender = await _unitOfWork.Users.GetByIdAsync(senderId);
            if (sender is null)
            {
                return ServiceResponse<MessageDto>.Failure("Sender not found.");
            }

            // 2. Verify receiver exists
            var receiver = await _unitOfWork.Users.GetByIdAsync(dto.ReceiverId);
            if (receiver is null)
            {
                return ServiceResponse<MessageDto>.Failure("Receiver not found.");
            }

            // 3. Prevent sending message to self
            if (senderId == dto.ReceiverId)
            {
                return ServiceResponse<MessageDto>.Failure("You cannot send a message to yourself.");
            }

            // 4. Find or create conversation between sender and receiver
            var conversation = await FindOrCreateConversationAsync(senderId, dto.ReceiverId);

            // 5. Create the message
            var message = new Message
            {
                ConversationID = conversation.ConversationID,
                SenderID = senderId,
                MessageText = dto.Content,
                MessageType = MessageType.Text,
                IsRead = false,
                IsEdited = false,
                SentAt = DateTime.UtcNow
            };

            await _unitOfWork.Messages.AddAsync(message);

            // 6. Update conversation's last message timestamp
            conversation.LastMessageAt = message.SentAt;
            _unitOfWork.Conversations.Update(conversation);

            await _unitOfWork.SaveAsync();

            // 7. Get sender's display name
            var senderName = await GetUserDisplayNameAsync(senderId);

            // 8. Build message DTO
            var messageDto = new MessageDto
            {
                Id = message.MessageID,
                ConversationId = conversation.ConversationID,
                SenderId = senderId,
                SenderName = senderName,
                Content = message.MessageText,
                SentAt = message.SentAt,
                IsRead = false
            };

            // 9. Send real-time message to receiver
            await _notifier.SendMessageToUserAsync(dto.ReceiverId, messageDto);

            return ServiceResponse<MessageDto>.Success(messageDto, "Message sent successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<MessageDto>.Failure(
                "An error occurred while sending the message.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<ConversationDto>>> GetMyConversationsAsync(int userId)
    {
        try
        {
            // 1. Verify user exists
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return ServiceResponse<IEnumerable<ConversationDto>>.Failure("User not found.");
            }

            // 2. Get all conversation participations for this user
            var participations = await _unitOfWork.ConversationParticipants
                .FindAsync(p => p.UserID == userId);

            var conversationDtos = new List<ConversationDto>();

            foreach (var participation in participations)
            {
                // Get the conversation
                var conversation = await _unitOfWork.Conversations.GetByIdAsync(participation.ConversationID);
                if (conversation is null) continue;

                // Only handle direct conversations
                if (conversation.ConversationType != ConversationType.Direct) continue;

                // Find the other participant
                var otherParticipation = await _unitOfWork.ConversationParticipants
                    .FindSingleAsync(p => p.ConversationID == conversation.ConversationID && p.UserID != userId);

                if (otherParticipation is null) continue;

                var otherUser = await _unitOfWork.Users.GetByIdAsync(otherParticipation.UserID);
                if (otherUser is null) continue;

                // Get last message
                var messages = await _unitOfWork.Messages
                    .FindAsync(m => m.ConversationID == conversation.ConversationID);
                var lastMessage = messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                // Count unread messages (messages not sent by user and not read)
                var unreadCount = messages.Count(m => m.SenderID != userId && !m.IsRead);

                // Get other user's display name and profile picture
                var (otherUserName, profilePicture) = await GetUserDisplayNameAndPictureAsync(otherParticipation.UserID);

                conversationDtos.Add(new ConversationDto
                {
                    ConversationId = conversation.ConversationID,
                    OtherUserId = otherParticipation.UserID,
                    OtherUserName = otherUserName,
                    OtherUserProfilePicture = profilePicture,
                    LastMessage = lastMessage?.MessageText,
                    LastMessageAt = lastMessage?.SentAt ?? conversation.CreatedAt,
                    UnreadCount = unreadCount
                });
            }

            // Order by last message time (most recent first)
            var orderedConversations = conversationDtos
                .OrderByDescending(c => c.LastMessageAt)
                .ToList();

            return ServiceResponse<IEnumerable<ConversationDto>>.Success(orderedConversations);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<ConversationDto>>.Failure(
                "An error occurred while retrieving conversations.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<IEnumerable<MessageDto>>> GetConversationMessagesAsync(int userId, int conversationId)
    {
        try
        {
            // 1. Verify user is a participant in this conversation (Security)
            var participation = await _unitOfWork.ConversationParticipants
                .FindSingleAsync(p => p.ConversationID == conversationId && p.UserID == userId);

            if (participation is null)
            {
                return ServiceResponse<IEnumerable<MessageDto>>.Failure(
                    "You are not a participant in this conversation.");
            }

            // 2. Get all messages for this conversation
            var messages = await _unitOfWork.Messages
                .FindAsync(m => m.ConversationID == conversationId);

            var messageDtos = new List<MessageDto>();

            foreach (var message in messages.OrderBy(m => m.SentAt))
            {
                var senderName = await GetUserDisplayNameAsync(message.SenderID);

                messageDtos.Add(new MessageDto
                {
                    Id = message.MessageID,
                    ConversationId = message.ConversationID,
                    SenderId = message.SenderID,
                    SenderName = senderName,
                    Content = message.MessageText,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead
                });
            }

            return ServiceResponse<IEnumerable<MessageDto>>.Success(messageDtos);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<MessageDto>>.Failure(
                "An error occurred while retrieving messages.",
                new List<string> { ex.Message });
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResponse<bool>> MarkConversationAsReadAsync(int userId, int conversationId)
    {
        try
        {
            // 1. Verify user is a participant
            var participation = await _unitOfWork.ConversationParticipants
                .FindSingleAsync(p => p.ConversationID == conversationId && p.UserID == userId);

            if (participation is null)
            {
                return ServiceResponse<bool>.Failure(
                    "You are not a participant in this conversation.");
            }

            // 2. Get all unread messages not sent by this user
            var unreadMessages = await _unitOfWork.Messages
                .FindAsync(m => m.ConversationID == conversationId && 
                               m.SenderID != userId && 
                               !m.IsRead);

            // 3. Mark them as read
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                _unitOfWork.Messages.Update(message);
            }

            // 4. Update participant's last read timestamp
            participation.LastReadAt = DateTime.UtcNow;
            _unitOfWork.ConversationParticipants.Update(participation);

            await _unitOfWork.SaveAsync();

            return ServiceResponse<bool>.Success(true, "Conversation marked as read.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure(
                "An error occurred while marking conversation as read.",
                new List<string> { ex.Message });
        }
    }

    /// <summary>
    /// Finds an existing direct conversation between two users or creates a new one.
    /// </summary>
    private async Task<Conversation> FindOrCreateConversationAsync(int user1Id, int user2Id)
    {
        // Find existing direct conversation between these two users
        var user1Participations = await _unitOfWork.ConversationParticipants
            .FindAsync(p => p.UserID == user1Id);

        foreach (var participation in user1Participations)
        {
            var conversation = await _unitOfWork.Conversations.GetByIdAsync(participation.ConversationID);
            if (conversation is null || conversation.ConversationType != ConversationType.Direct)
                continue;

            // Check if user2 is also a participant
            var user2Participation = await _unitOfWork.ConversationParticipants
                .FindSingleAsync(p => p.ConversationID == conversation.ConversationID && p.UserID == user2Id);

            if (user2Participation is not null)
            {
                // Found existing conversation
                return conversation;
            }
        }

        // No existing conversation found - create a new one
        var newConversation = new Conversation
        {
            ConversationType = ConversationType.Direct,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Conversations.AddAsync(newConversation);
        await _unitOfWork.SaveAsync();

        // Add both participants
        var participant1 = new ConversationParticipant
        {
            ConversationID = newConversation.ConversationID,
            UserID = user1Id,
            JoinedAt = DateTime.UtcNow
        };

        var participant2 = new ConversationParticipant
        {
            ConversationID = newConversation.ConversationID,
            UserID = user2Id,
            JoinedAt = DateTime.UtcNow
        };

        await _unitOfWork.ConversationParticipants.AddAsync(participant1);
        await _unitOfWork.ConversationParticipants.AddAsync(participant2);
        await _unitOfWork.SaveAsync();

        return newConversation;
    }

    /// <summary>
    /// Gets the display name for a user (student name or company name).
    /// </summary>
    private async Task<string> GetUserDisplayNameAsync(int userId)
    {
        // Check if user is a student
        var student = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == userId);
        if (student is not null)
        {
            return student.FullName;
        }

        // Check if user is a company
        var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == userId);
        if (company is not null)
        {
            return company.CompanyName;
        }

        // Fallback to user email
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        return user?.Email ?? "Unknown User";
    }

    /// <summary>
    /// Gets the display name and profile picture for a user.
    /// </summary>
    private async Task<(string Name, string? ProfilePicture)> GetUserDisplayNameAndPictureAsync(int userId)
    {
        // Check if user is a student
        var student = await _unitOfWork.Students.FindSingleAsync(s => s.UserID == userId);
        if (student is not null)
        {
            return (student.FullName, student.ProfilePicture);
        }

        // Check if user is a company
        var company = await _unitOfWork.Companies.FindSingleAsync(c => c.UserID == userId);
        if (company is not null)
        {
            return (company.CompanyName, company.CompanyLogo);
        }

        // Fallback to user email
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        return (user?.Email ?? "Unknown User", null);
    }
}
