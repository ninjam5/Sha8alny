namespace Sh8lny.Domain.Models;

// Master catalog of available skills
public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Many-to-many relationship with students
    public ICollection<StudentSkill> StudentSkills { get; set; } = new HashSet<StudentSkill>();
}
