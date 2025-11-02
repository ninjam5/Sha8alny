using System.Collections;

namespace Sh8lny.Domain.Models;

// Core user entity for both Students and Companies
public class User
{
    // Primary key
    public int Id { get; set; }
    
    // User credentials and basic info
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; } // "Student" or "Company"
    
    // Optional profile information
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime Created_At { get; set; }

    // One-to-one relationships with profile types
    public StudentProfile StudentProfile { get; set; } = null!;
    public CompanyProfile CompanyProfile { get; set; } = null!;

    // Collections for related entities
    public ICollection<Notification> Notification { get; set; } = new HashSet<Notification>();
    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
    public ICollection<Payment> SentPayments { get; set; } = new HashSet<Payment>();
    public ICollection<Payment> ReceivedPayments { get; set; } = new HashSet<Payment>();
    public ICollection<Review> ReviewsWritten { get; set; } = new HashSet<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new HashSet<Review>();
}
