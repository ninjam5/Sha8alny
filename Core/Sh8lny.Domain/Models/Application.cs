namespace Sh8lny.Domain.Models;

public class Application
{
    public int Id { get; set; }
    public required string Status { get; set; }
    public DateTime Created_At { get; set; }
    public string? CV { get; set; }
    public string? Proposal { get; set; }
    public string? Notes { get; set; }

    //Navigation Properties
    public int StudentId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public int OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
}
