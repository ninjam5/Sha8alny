using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; }

    [Column(TypeName = "text")]
    public string Body { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
