using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class University
    {
        // Primary key
        public int UniversityID { get; set; }

        // Foreign key
        public int? UserID { get; set; }

        // Basic info
        public required string UniversityName { get; set; }
        public string? UniversityLogo { get; set; }

        // Contact info
        public required string ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Website { get; set; }

        // Location
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        // University details
        public UniversityType? UniversityType { get; set; }
        public bool IsActive { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }

        // Collections
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<DashboardMetric> DashboardMetrics { get; set; } = new List<DashboardMetric>();
    }

    /// <summary>
    /// University type enumeration
    /// </summary>
    public enum UniversityType
    {
        Public,
        Private,
        International
    }
}
