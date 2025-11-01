namespace Sh8lny.Domain.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public bool Is_Read { get; set; } = false;
    public DateTime Created_At { get; set; }

    //Navigation Property
    public int UId { get; set; }
    public User User { get; set; } = null!;
}
