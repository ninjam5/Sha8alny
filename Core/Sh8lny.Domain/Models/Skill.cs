namespace Sh8lny.Domain.Models;

/// <summary>
/// Skills master entity
/// </summary>
public class Skill
{
    // Primary key
    public int SkillID { get; set; }

    // Skill details
    public required string SkillName { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Collections
    public ICollection<StudentSkill> StudentSkills { get; set; } = new List<StudentSkill>();
    public ICollection<ProjectRequiredSkill> ProjectRequiredSkills { get; set; } = new List<ProjectRequiredSkill>();
}

/// <summary>
/// Skill category enumeration
/// </summary>
public enum SkillCategory
{
    Backend,
    Frontend,
    UIUX,
    Mobile,
    AIML,
    Data,
    Testing,
    Marketing,
    Design,
    Security,
    Other
}
