namespace Sh8lny.Domain.Entities;

/// <summary>
/// Group member entity (junction table for ProjectGroup-Student)
/// </summary>
public class GroupMember
{
    // Primary key
    public int GroupMemberID { get; set; }

    // Foreign keys
    public int GroupID { get; set; }
    public int StudentID { get; set; }

    // Member details
    public string? Role { get; set; }

    // Timestamp
    public DateTime JoinedAt { get; set; }

    // Navigation properties
    public ProjectGroup ProjectGroup { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
