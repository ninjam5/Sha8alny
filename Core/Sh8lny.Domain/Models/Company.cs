using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class Company
    {
        // Primary key
        public int CompanyID { get; set; }

        // Foreign key to User
        public int UserID { get; set; }

        // Basic info
        public required string CompanyName { get; set; }
        public string? CompanyLogo { get; set; }

        // Contact info
        public required string ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }

        // Location
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }

        // Company details
        public string? Industry { get; set; }
        public string? Description { get; set; }

        /* Partnership [Removed this shit]
        public bool IsVerified { get; set; }
        //public PartnershipTier? PartnershipTier { get; set; }
        public DateTime? PartnershipStartDate { get; set; }
        public CompanyStatus Status { get; set; }
        */

        // Rating & Reviews
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;

        // Collections
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<Certificate> IssuedCertificates { get; set; } = new HashSet<Certificate>();
        public ICollection<DashboardMetric> DashboardMetrics { get; set; } = new HashSet<DashboardMetric>();
        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public ICollection<CompanyReview> Reviews { get; set; } = new HashSet<CompanyReview>();
        public ICollection<StudentReview> StudentReviews { get; set; } = new HashSet<StudentReview>();
    }

    public enum CompanyStatus
    {
        Active,
        Inactive,
        PendingApproval,
        Suspended
    }
}

