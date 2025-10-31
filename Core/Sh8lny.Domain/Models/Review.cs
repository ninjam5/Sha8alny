namespace Sh8lny.Domain.Models;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Property

    public int UIdReviewer { get; set; }
    public User Reviewer { get; set; }

    public int UIdTarget { get; set; }
    public User Target { get; set; }
    public int Completed_id { get; set; }
    public CompletedOpportunity CompletedOpportunity { get; set; }

}
