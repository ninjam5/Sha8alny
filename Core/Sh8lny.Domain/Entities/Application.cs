using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Application
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "text")]
    public string? CoverLetter { get; set; }
    
    [StringLength(50)]
    public string? Status { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public int Student_ID { get; set; }

    public int Opportunity_ID { get; set; }
}
