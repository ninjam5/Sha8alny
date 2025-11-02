namespace Sh8lny.Domain.Models;

// Junction table for Student-Skill many-to-many relationship
public class StudentSkill
{
    // Composite primary key
    public int StudentId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;

    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
