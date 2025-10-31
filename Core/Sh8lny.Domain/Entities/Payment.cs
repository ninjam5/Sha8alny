using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sh8lny.Domain.Entities;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(10)]
    public string Currency { get; set; } // e.g., "USD"

    [StringLength(50)]
    public string Type { get; set; } // e.g., "Milestone", "Final"

    [StringLength(50)]
    public string Method { get; set; } // e.g., "Stripe", "PayPal"

    [StringLength(50)]
    public string Status { get; set; } // e.g., "Pending", "Complete"

    public DateTime? PaidAt { get; set; }

    public int TotalInstallments { get; set; }

    public int InstallmentNumber { get; set; }

    public bool Completed { get; set; }
}
