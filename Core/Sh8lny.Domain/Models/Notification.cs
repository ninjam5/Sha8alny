namespace Sh8lny.Domain.Models;

// System notifications to users
public class Notification
{
    public int Id { get; set; }
    
    // Notification content
    public required string Title { get; set; }
    public required string Body { get; set; }
    public bool Is_Read { get; set; } = false;
    public DateTime Created_At { get; set; }

    // Recipient relationship
    public int UId { get; set; }
    public User User { get; set; } = null!;
}