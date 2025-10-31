namespace Sh8lny.Domain.Models;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public bool Is_Read { get; set; } = false;
    public DateTime Created_At { get; set; }

    //Navigation Property
    public int UId { get; set; }
    public User User { get; set; }
}
