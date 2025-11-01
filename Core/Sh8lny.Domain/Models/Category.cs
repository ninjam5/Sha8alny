namespace Sh8lny.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Icon_URL { get; set; }
    public required string Description { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Properties
    public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();
}
