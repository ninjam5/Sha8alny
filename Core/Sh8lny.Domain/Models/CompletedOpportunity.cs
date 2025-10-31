namespace Sh8lny.Domain.Models;

public class CompletedOpportunity
{
    public int Id { get; set; }
    public bool ConfirmedByStudent { get; set; }
    public bool ConfirmedByCompany { get; set; }
    public bool ConfirmedByPayment { get; set; }
    public DateTime Completed_At { get; set; }

    //Navigation Properties
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; }
    public int OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; }
    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
