namespace Sh8lny.Domain.Models;

// Tracks successfully completed opportunities
public class CompletedOpportunity
{
    public int Id { get; set; }
    
    // Completion confirmations
    public bool ConfirmedByStudent { get; set; }
    public bool ConfirmedByCompany { get; set; }
    public bool ConfirmedByPayment { get; set; }
    public DateTime Completed_At { get; set; }

    // Foreign key relationships
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public int OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
    
    // Associated payments and reviews
    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
