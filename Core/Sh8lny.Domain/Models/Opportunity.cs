namespace Sh8lny.Domain.Models;

public class Opportunity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public required string Requirements { get; set; }
    public DateTime Deadline { get; set; }
    public bool Is_Paid { get; set; }
    public decimal Payment { get; set; }
    public required string Duration { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Properties
    public int CompanyProfileId { get; set; }
    public CompanyProfile CompanyProfile { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public CompletedOpportunity? CompletedOpportunity { get; set; }
    }
