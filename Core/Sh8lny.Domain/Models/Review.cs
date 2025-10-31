namespace Sh8lny.Domain.Models;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public DateTime Created_At { get; set; }
}
