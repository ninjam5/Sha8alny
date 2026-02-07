using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }

        // --- Internal Links ---
        public int ProjectID { get; set; }
        public int StudentID { get; set; }
        public int? CompanyID { get; set; }

        // --- Financials ---
        public decimal Amount { get; set; } // Amount in CENTS (Paymob usually expects cents) or standard unit
        public required string Currency { get; set; } // E.g., "EGP"
        public PaymentStatus Status { get; set; }

        // --- Paymob Specific Fields (CRITICAL) ---

        /// <summary>
        /// The Order ID returned by Paymob's "Order Registration API"
        /// </summary>
        public string? PaymobOrderId { get; set; }

        /// <summary>
        /// The Transaction ID returned by Paymob after the user pays (via Webhook)
        /// </summary>
        public string? PaymobTransactionId { get; set; }

        /// <summary>
        /// Stores the raw JSON response from Paymob for debugging disputes
        /// </summary>
        public string? GatewayRawResponse { get; set; }

        // --- Metadata ---
        public PaymentMethod PaymentMethod { get; set; } // Card, Wallet, Kiosk
        public string? Description { get; set; }

        // --- Timestamps ---
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public Company? Company { get; set; }
    }

    /// <summary>
    /// Payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Refunded,
        Cancelled
    }

    /// <summary>
    /// Payment method enumeration
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        PayPal,
        Stripe,
        Cash,
        Wallet,
        Other
    }
}
