using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class CompletedOpportunity
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
    
    public string? Submission { get; set; }
    
    public int? Rating { get; set; } 

    public int Student_ID { get; set; }

    public int Opportunity_ID { get; set; }
}
