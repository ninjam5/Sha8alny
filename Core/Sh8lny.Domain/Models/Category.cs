namespace Sh8lny.Domain.Models;

// Categorizes opportunities (e.g., Technology, Marketing)
public class Category
{
    public int Id { get; set; }
    
    // Category details
    public required string Name { get; set; }
    public required string Icon_URL { get; set; }
    public required string Description { get; set; }
    public DateTime Created_At { get; set; }

    // Associated opportunities
    public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();
}
