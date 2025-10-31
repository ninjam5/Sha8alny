namespace Sh8lny.Domain.Models;

public class Opportunity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public DateTime Deadline { get; set; }
    public bool Is_Paid { get; set; }
    public decimal Payment { get; set; }
    public string Duration { get; set; }
    public DateTime Created_At { get; set; }
    public string Library { get; set; }
}
