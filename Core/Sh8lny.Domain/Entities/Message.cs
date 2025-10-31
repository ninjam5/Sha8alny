using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }
}
