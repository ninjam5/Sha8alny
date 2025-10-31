using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

/// <summary>
/// A review written BY a User (U_id) ABOUT either a 
/// CompanyProfile (cId) OR a StudentProfile (std).
/// </summary>
public class Review
{
    [Key]
    public int Id { get; set; }

    public int Rating { get; set; } // 1-5

    [Column(TypeName = "text")]
    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}
