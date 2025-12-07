using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class Department
    {
        // Primary key
        public int DepartmentID { get; set; }

        // Foreign key
        public int UniversityID { get; set; }

        // Department details
        public required string DepartmentName { get; set; }
        //public string? DepartmentCode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        //public University University { get; set; } = null!;

        // Collections
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
