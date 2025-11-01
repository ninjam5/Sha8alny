namespace Sh8lny.Domain.Models;

public class StudentProfile
{
    public int Id { get; set; }
    public required string University { get; set; }
    public required string Major { get; set; }
    public DateTime GraduationYear { get; set; }
    public int TrainingDays { get; set; }

    public string? CV { get; set; }

    //Navigation Properties
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public ICollection<StudentSkill> StudentSkills { get; set; } = new HashSet<StudentSkill>();
    public ICollection<CompletedOpportunity> CompletedOpportunities { get; set; } = new HashSet<CompletedOpportunity>();
}
