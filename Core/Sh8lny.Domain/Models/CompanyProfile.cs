namespace Sh8lny.Domain.Models;

public class CompanyProfile
{
    public int Id { get; set; }
    public string? WebSite { get; set; }
    public required string Industry { get; set; }


    //Navigation Properties
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<Opportunity> Opportunities { get; set; } = new HashSet<Opportunity>();

}
