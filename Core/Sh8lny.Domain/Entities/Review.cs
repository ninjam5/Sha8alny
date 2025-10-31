using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }
    
    public int Rating { get; set; } 
    
    [Column(TypeName = "text")]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public int User_ID { get; set; }

    public int? Company_ID { get; set; }

    public int? Student_ID { get; set; }
}
