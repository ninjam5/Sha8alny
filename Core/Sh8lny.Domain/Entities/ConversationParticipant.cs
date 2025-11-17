namespace Sh8lny.Domain.Entities;

/// <summary>
/// Conversation participant entity (junction table)
/// </summary>
public class ConversationParticipant
{
    // Primary key
    public int ParticipantID { get; set; }

    // Foreign keys
    public int ConversationID { get; set; }
    public int UserID { get; set; }

    // Timestamps
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }

    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}
