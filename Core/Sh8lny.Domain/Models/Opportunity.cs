namespace Sh8lny.Domain.Models;

public class Opportunity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public DateTime Deadline { get; set; }
    public bool Is_Paid { get; set; }
    public decimal Payment { get; set; }
    public string Duration { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Properties
    public int CompanyProfileId { get; set; }
    public CompanyProfile CompanyProfile { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    public CompletedOpportunity CompletedOpportunity { get; set; }
    }
