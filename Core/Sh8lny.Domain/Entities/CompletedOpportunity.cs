using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Domain.Entities;

/// <summary>
/// Represents an opportunity that has been finished by a student.
/// This is the "join" table that links Students and Opportunities.
/// </summary>
public class CompletedOpportunity
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Status { get; set; } // e.g., "In-Progress", "Submitted"

    public string Submission { get; set; } // URL to work, file path

    public int? Rating { get; set; } 
}
