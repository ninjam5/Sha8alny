namespace Sh8lny.Domain.Models;

/// <summary>
/// Central authentication and user management entity
/// </summary>
public class User
{
    // Primary key
    public int UserID { get; set; }

    // Authentication
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserType UserType { get; set; }
    
    // Verification
    public bool IsEmailVerified { get; set; }
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }
    
    // Status and timestamps
    //public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // One-to-one navigation properties
    public Student? Student { get; set; }
    public Company? Company { get; set; }
    //public University? University { get; set; }
    public UserSettings? Settings { get; set; }
    
    // Collections
    public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new HashSet<ConversationParticipant>();
    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    //public ICollection<ActivityLog> ActivityLogs { get; set; } = new HashSet<ActivityLog>();
    public ICollection<CompletedOpportunity> VerifiedOpportunities { get; set; } = new HashSet<CompletedOpportunity>();
}

/// <summary>
/// User type enumeration
/// </summary>
public enum UserType
{
    Student,
    Company,
    //University,
    Admin
}
