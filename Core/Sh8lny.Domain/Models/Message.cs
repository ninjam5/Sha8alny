namespace Sh8lny.Domain.Models;

// Direct messaging between users
public class Message
{
    public int Id { get; set; }
    
    // Message content
    public required string Content { get; set; }
    public DateTime Created_At { get; set; }

    // Sender and receiver relationships
    public int UIdSender { get; set; }
    public User Sender { get; set; } = null!;
    public int UIdReceiver { get; set; }
    public User Receiver { get; set; } = null!;
}
