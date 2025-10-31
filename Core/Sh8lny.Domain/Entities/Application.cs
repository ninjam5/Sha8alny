using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;
public class Application
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "text")]
    public string CoverLetter { get; set; }

    [StringLength(50)]
    public string Status { get; set; } // e.g., "Pending", "Accepted"

    public DateTime CreatedAt { get; set; }
}
