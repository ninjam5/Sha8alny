using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [StringLength(50)]
    public string Role { get; set; } = string.Empty; 

    public string? ProfilePicture { get; set; }
    
    [Column(TypeName = "text")]
    public string? Bio { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
