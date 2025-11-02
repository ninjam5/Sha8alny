namespace Sh8lny.Domain.Models;

// Student application to an opportunity
public class Application
{
    public int Id { get; set; }
    
    // Application details
    public required string Status { get; set; } // e.g., "Pending", "Accepted", "Rejected"
    public DateTime Created_At { get; set; }
    public required string CV { get; set; }
    public required string Proposal { get; set; }
    public required string Notes { get; set; }

    // Foreign key relationships
    public int StudentId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public int OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
}
