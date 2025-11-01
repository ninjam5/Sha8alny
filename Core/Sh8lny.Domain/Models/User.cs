using System.Collections;

namespace Sh8lny.Domain.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime Created_At { get; set; }

    //Navigation Properties
    public StudentProfile? StudentProfile { get; set; }
    public CompanyProfile? CompanyProfile { get; set; }

    public ICollection<Notification> Notification { get; set; } = new HashSet<Notification>();
    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
    public ICollection<Payment> SentPayments { get; set; } = new HashSet<Payment>();
    public ICollection<Payment> ReceivedPayments { get; set; } = new HashSet<Payment>();
    public ICollection<Review> ReviewsWritten { get; set; } = new HashSet<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new HashSet<Review>();

}
