namespace Sh8lny.Domain.Models;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Type { get; set; }
    public string Method { get; set; }
    public string Status { get; set; }
    public DateTime Paid_At { get; set; }
    public int Total_Installment { get; set; }
    public int Installment_Number { get; set; }

    //Navigation Property
    public int UIdSender { get; set; }
    public User Sender { get; set; }
    public int UIdReceiver { get; set; }
    public User Receiver { get; set; }
    public int Completed_id { get; set; }
    public CompletedOpportunity CompletedOpportunity { get; set; } 
}
