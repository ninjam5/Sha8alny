namespace Sh8lny.Domain.Models;

// Financial transaction between users
public class Payment
{
    public int Id { get; set; }
    
    // Payment details
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string Type { get; set; } // e.g., "Full", "Installment"
    public required string Method { get; set; } // e.g., "Credit Card", "Bank Transfer"
    public required string Status { get; set; } // e.g., "Pending", "Completed"
    public DateTime Paid_At { get; set; }
    
    // Installment tracking
    public int Total_Installment { get; set; }
    public int Installment_Number { get; set; }

    // Foreign key relationships
    public int UIdSender { get; set; }
    public User Sender { get; set; } = null!;
    public int UIdReceiver { get; set; }
    public User Receiver { get; set; } = null!;
    public int Completed_id { get; set; }
    public CompletedOpportunity CompletedOpportunity { get; set; } = null!;
}
