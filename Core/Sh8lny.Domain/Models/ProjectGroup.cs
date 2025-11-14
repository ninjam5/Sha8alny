namespace Sh8lny.Domain.Models;

/// <summary>
/// Project group/team entity
/// </summary>
public class ProjectGroup
{
    // Primary key
    public int GroupID { get; set; }

    // Foreign key
    public int ProjectID { get; set; }

    // Group details
    public required string GroupName { get; set; }
    public string? Description { get; set; }
    public int? MaxMembers { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    
    // Collections
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
