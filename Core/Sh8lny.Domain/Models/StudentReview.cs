using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class StudentReview
    {
        // Primary key
        public int ReviewID { get; set; }

        // Foreign keys
        public int StudentID { get; set; }
        public int CompanyID { get; set; }
        public int? CompletedOpportunityID { get; set; }
        public int? ProjectID { get; set; }
        public int? ApplicationID { get; set; }

        // Review details
        public decimal Rating { get; set; }
        public string? ReviewTitle { get; set; }
        public string? ReviewText { get; set; }

        // Detailed ratings (optional breakdown)
        public decimal? TechnicalSkillsRating { get; set; }
        public decimal? CommunicationRating { get; set; }
        public decimal? ProfessionalismRating { get; set; }
        public decimal? TimeManagementRating { get; set; }
        public decimal? TeamworkRating { get; set; }
        public decimal? ProblemSolvingRating { get; set; }

        // Recommendations
        public bool WouldHireAgain { get; set; }
        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }

        // Status
        public ReviewStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPublic { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Student response
        public string? StudentResponse { get; set; }
        public DateTime? StudentRespondedAt { get; set; }

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public CompletedOpportunity? CompletedOpportunity { get; set; }
    }
}
