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
    public string Transaction_Id { get; set; }
}
