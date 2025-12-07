using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class DashboardMetric
    {
        // Primary key
        public int MetricID { get; set; }

        // Foreign keys
        //public int? CompanyID { get; set; }
        //public int? UniversityID { get; set; }

        // Metrics
        public int TotalStudents { get; set; }
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int AvailableOpportunities { get; set; }
        public int NewApplicants { get; set; }
        //public int ProjectsNearDeadline { get; set; }
        public decimal ActivityIncreasePercent { get; set; }

        // Timestamps
        public DateTime MetricDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        // public Company? Company { get; set; }
        //public University? University { get; set; }
    }
}
