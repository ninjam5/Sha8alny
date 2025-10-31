namespace Sh8lny.Domain.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Property
    public int UIdSender { get; set; }
    public User Sender { get; set; }
    public int UIdReceiver { get; set; }
    public User Receiver { get; set; }
}
