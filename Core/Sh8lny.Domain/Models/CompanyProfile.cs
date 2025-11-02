namespace Sh8lny.Domain.Models;

// Extended profile for company users
public class CompanyProfile
{
    public int Id { get; set; }
    
    // Company details
    public string? WebSite { get; set; }
    public required string Industry { get; set; }

    // One-to-one relationship with User
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Company's posted opportunities
    public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();
}
