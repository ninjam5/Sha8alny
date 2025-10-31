using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

/// <summary>
/// Central user table. Can be a student or a company, determined by Role
/// and the existence of a related StudentProfile or CompanyProfile.
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; } // Remember to hash this!

    [StringLength(50)]
    public string Role { get; set; } // e.g., "Student", "Company"

    public string ProfilePicture { get; set; } // URL or file path

    [Column(TypeName = "text")]
    public string Bio { get; set; }

    public DateTime CreatedAt { get; set; }
}
