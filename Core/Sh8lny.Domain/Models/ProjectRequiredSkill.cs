namespace Sh8lny.Domain.Models;

/// <summary>
/// Junction table: Project-Skill many-to-many relationship
/// </summary>
public class ProjectRequiredSkill
{
    // Primary key
    public int ProjectSkillID { get; set; }

    // Foreign keys
    public int ProjectID { get; set; }
    public int SkillID { get; set; }

    // Requirement level
    public bool IsRequired { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Project Project { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
