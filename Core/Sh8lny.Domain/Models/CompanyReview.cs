using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class CompanyReview
    {
        // Primary key
        public int ReviewID { get; set; }

        // Foreign keys
        public int CompanyID { get; set; }
        public int StudentID { get; set; }
        public int? CompletedOpportunityID { get; set; }

        // Review details
        public decimal Rating { get; set; }
        public string? ReviewTitle { get; set; }
        public string? ReviewText { get; set; }

        // Detailed ratings (optional breakdown)
        public decimal? WorkEnvironmentRating { get; set; }
        public decimal? LearningOpportunityRating { get; set; }
        public decimal? MentorshipRating { get; set; }
        public decimal? CompensationRating { get; set; }
        public decimal? CommunicationRating { get; set; }

        // Recommendations
        public bool WouldRecommend { get; set; }
        public string? Pros { get; set; }
        public string? Cons { get; set; }

        // Status
        public ReviewStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAnonymous { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Company response
        public string? CompanyResponse { get; set; }
        public DateTime? CompanyRespondedAt { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public CompletedOpportunity? CompletedOpportunity { get; set; }
    }

    /// <summary>
    /// Review status enumeration
    /// </summary>
    public enum ReviewStatus
    {
        Pending,
        Approved,
        Rejected,
        Flagged
    }
}

