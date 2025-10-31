using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

/// <summary>
/// Feedback survey about a specific CompletedOpportunity.
/// </summary>
public class Survey
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "text")]
    public string UserFeedback { get; set; }

    [Column(TypeName = "text")]
    public string CompanyFeedback { get; set; }

    public int PlatformRating { get; set; } // 1-5
}
