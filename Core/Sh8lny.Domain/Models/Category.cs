namespace Sh8lny.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Icon_URL { get; set; }
    public string Description { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Properties
    public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();
}
