namespace Sh8lny.Domain.Models;

public class StudentSkill
{
    public int StudentId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;

    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
