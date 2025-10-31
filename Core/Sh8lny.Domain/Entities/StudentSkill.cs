using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Domain.Entities;

public class StudentSkill
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string SkillName { get; set; }
}
