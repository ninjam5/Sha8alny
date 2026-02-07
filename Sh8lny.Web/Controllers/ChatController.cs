using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Abstraction.Services;
using Sh8lny.Shared.DTOs.Chat;
using Sh8lny.Shared.DTOs.Common;

namespace Sh8lny.Web.Controllers;

/// <summary>
/// Controller for chat operations between users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Sends a message to another user.
    /// </summary>
    /// <param name="dto">The message data.</param>
    /// <returns>The sent message.</returns>
    [HttpPost("send")]
    public async Task<ActionResult<ServiceResponse<MessageDto>>> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<MessageDto>.Failure("Invalid or missing user token."));
        }

        var result = await _chatService.SendMessageAsync(userId.Value, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all conversations for the current user.
    /// </summary>
    /// <returns>List of conversations.</returns>
    [HttpGet("conversations")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<ConversationDto>>>> GetMyConversations()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<IEnumerable<ConversationDto>>.Failure("Invalid or missing user token."));
        }

        var result = await _chatService.GetMyConversationsAsync(userId.Value);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets all messages in a specific conversation.
    /// </summary>
    /// <param name="id">The conversation ID.</param>
    /// <returns>List of messages.</returns>
    [HttpGet("conversations/{id}/messages")]
    public async Task<ActionResult<ServiceResponse<IEnumerable<MessageDto>>>> GetConversationMessages(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<IEnumerable<MessageDto>>.Failure("Invalid or missing user token."));
        }

        var result = await _chatService.GetConversationMessagesAsync(userId.Value, id);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Marks all messages in a conversation as read.
    /// </summary>
    /// <param name="id">The conversation ID.</param>
    /// <returns>Success or failure response.</returns>
    [HttpPut("conversations/{id}/read")]
    public async Task<ActionResult<ServiceResponse<bool>>> MarkConversationAsRead(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized(ServiceResponse<bool>.Failure("Invalid or missing user token."));
        }

        var result = await _chatService.MarkConversationAsReadAsync(userId.Value, id);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Extracts the current user ID from JWT claims.
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }
        return userId;
    }
}
