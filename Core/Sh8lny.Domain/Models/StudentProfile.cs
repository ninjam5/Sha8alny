namespace Sh8lny.Domain.Models;

// Extended profile for student users
public class StudentProfile
{
    public int Id { get; set; }
    
    // Academic information
    public required string University { get; set; }
    public required string Major { get; set; }
    public DateTime GraduationYear { get; set; }
    public int TrainingDays { get; set; }
    public required string CV { get; set; }

    // One-to-one relationship with User
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    // Collections for related entities
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public ICollection<StudentSkill> StudentSkills { get; set; } = new HashSet<StudentSkill>();
    public ICollection<CompletedOpportunity> CompletedOpportunities { get; set; } = new HashSet<CompletedOpportunity>();
}
