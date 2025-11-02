namespace Sh8lny.Domain.Models;

// Job/internship posting by companies
public class Opportunity
{
    public int Id { get; set; }
    
    // Opportunity details
    public required string Title { get; set; }
    public required string Type { get; set; } // e.g., "Internship", "Job"
    public required string Description { get; set; }
    public required string Requirements { get; set; }
    public DateTime Deadline { get; set; }
    
    // Payment information
    public bool Is_Paid { get; set; }
    public decimal Payment { get; set; }
    
    // Additional details
    public required string Duration { get; set; }
    public DateTime Created_At { get; set; }

    // Foreign key relationships
    public int CompanyProfileId { get; set; }
    public CompanyProfile CompanyProfile { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    // Collections and one-to-one
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public CompletedOpportunity CompletedOpportunity { get; set; } = null!;
}
