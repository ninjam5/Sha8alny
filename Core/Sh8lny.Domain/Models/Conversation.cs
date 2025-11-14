namespace Sh8lny.Domain.Models;

/// <summary>
/// Conversation entity for messaging system
/// </summary>
public class Conversation
{
    // Primary key
    public int ConversationID { get; set; }

    // Conversation details
    public ConversationType ConversationType { get; set; }
    public int? GroupID { get; set; }
    public string? ConversationName { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    // Navigation properties
    public Project? Project { get; set; }
    public ProjectGroup? Group { get; set; }
    
    // Collections
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

/// <summary>
/// Conversation type enumeration
/// </summary>
public enum ConversationType
{
    Direct,
    Group,
}
