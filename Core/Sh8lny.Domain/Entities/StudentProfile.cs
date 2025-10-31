using System.ComponentModel.DataAnnotations;

namespace Sh8lny.Domain.Entities;

public class StudentProfile
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string University { get; set; }

    [StringLength(255)]
    public string Major { get; set; }

    public int GradYear { get; set; }
}
