using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Application.DTOs.Messaging;
using Sh8lny.Application.Interfaces;
using System.Security.Claims;

namespace Sh8lny.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagingController : ControllerBase
{
    private readonly IMessagingService _messagingService;
    private readonly ILogger<MessagingController> _logger;

    public MessagingController(IMessagingService messagingService, ILogger<MessagingController> logger)
    {
        _messagingService = messagingService;
        _logger = logger;
    }

    #region Conversation Endpoints

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var result = await _messagingService.CreateConversationAsync(dto);
        return CreatedAtAction(nameof(GetConversation), new { id = result.ConversationID }, result);
    }

    [HttpGet("conversations/{id}")]
    public async Task<IActionResult> GetConversation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.GetConversationByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetUserConversations()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.GetUserConversationsAsync(userId);
        return Ok(result);
    }

    [HttpPut("conversations/{id}")]
    public async Task<IActionResult> UpdateConversation(int id, [FromBody] UpdateConversationDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.UpdateConversationAsync(dto, userId);
        return Ok(result);
    }

    [HttpDelete("conversations/{id}")]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.DeleteConversationAsync(id, userId);
        return NoContent();
    }

    #endregion

    #region Participant Endpoints

    [HttpPost("conversations/{id}/participants")]
    public async Task<IActionResult> AddParticipants(int id, [FromBody] AddParticipantsDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.AddParticipantsAsync(dto, userId);
        return Ok();
    }

    [HttpDelete("conversations/{id}/participants")]
    public async Task<IActionResult> RemoveParticipant(int id, [FromBody] RemoveParticipantDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.RemoveParticipantAsync(dto, userId);
        return NoContent();
    }

    [HttpPost("conversations/{id}/leave")]
    public async Task<IActionResult> LeaveConversation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.LeaveConversationAsync(id, userId);
        return NoContent();
    }

    #endregion

    #region Message Endpoints

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.SendMessageAsync(dto, userId);
        return CreatedAtAction(nameof(GetMessage), new { id = result.MessageID }, result);
    }

    [HttpGet("messages/{id}")]
    public async Task<IActionResult> GetMessage(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.GetMessageByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetConversationMessages(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.GetConversationMessagesAsync(id, userId, page, pageSize);
        return Ok(result);
    }

    [HttpPut("messages/{id}")]
    public async Task<IActionResult> EditMessage(int id, [FromBody] EditMessageDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _messagingService.EditMessageAsync(dto, userId);
        return Ok(result);
    }

    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.DeleteMessageAsync(id, userId);
        return NoContent();
    }

    [HttpPost("conversations/{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, [FromBody] MarkAsReadDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _messagingService.MarkMessagesAsReadAsync(dto, userId);
        return Ok();
    }

    [HttpGet("conversations/{id}/unread-count")]
    public async Task<IActionResult> GetUnreadCount(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var count = await _messagingService.GetUnreadCountAsync(id, userId);
        return Ok(new { unreadCount = count });
    }

    #endregion
}
