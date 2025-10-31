using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class CompanyProfile
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string WebSite { get; set; }

    [StringLength(100)]
    public string Industry { get; set; }

    [Column(TypeName = "text")]
    public string Description { get; set; }
}
