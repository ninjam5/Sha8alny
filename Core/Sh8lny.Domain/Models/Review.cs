namespace Sh8lny.Domain.Models;

// User review and rating system
public class Review
{
    public int Id { get; set; }
    
    // Review content
    public int Rating { get; set; } // 1-5 stars
    public required string Comment { get; set; }
    public DateTime Created_At { get; set; }

    // Foreign key relationships - bidirectional reviews
    public int UIdReviewer { get; set; }
    public User Reviewer { get; set; } = null!;
    public int UIdTarget { get; set; }
    public User Target { get; set; } = null!;
    public int Completed_id { get; set; }
    public CompletedOpportunity CompletedOpportunity { get; set; } = null!;
}
