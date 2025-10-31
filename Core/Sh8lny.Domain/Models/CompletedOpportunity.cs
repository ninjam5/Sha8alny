namespace Sh8lny.Domain.Models;

public class CompletedOpportunity
{
    public int Id { get; set; }
    public string CompletedBy { get; set; }
    public string AcceptedBy { get; set; }
    public string SubmitContains { get; set; }
    public decimal Payment { get; set; }
    public DateTime Created_At { get; set; }
}
