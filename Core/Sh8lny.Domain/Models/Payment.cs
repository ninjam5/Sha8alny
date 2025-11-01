namespace Sh8lny.Domain.Models;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string Type { get; set; }
    public required string Method { get; set; }
    public required string Status { get; set; }
    public DateTime Paid_At { get; set; }
    public int Total_Installment { get; set; }
    public int Installment_Number { get; set; }

    //Navigation Property
    public int UIdSender { get; set; }
    public User Sender { get; set; } = null!;
    public int UIdReceiver { get; set; }
    public User Receiver { get; set; } = null!;
    public int Completed_id { get; set; }
    public CompletedOpportunity CompletedOpportunity { get; set; } = null!; 
}
