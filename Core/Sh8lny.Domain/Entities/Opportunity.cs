using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Opportunity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; }

    [StringLength(100)]
    public string Type { get; set; } // e.g., "Internship", "Freelance"

    [Column(TypeName = "text")]
    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    public bool Paid { get; set; }
}
