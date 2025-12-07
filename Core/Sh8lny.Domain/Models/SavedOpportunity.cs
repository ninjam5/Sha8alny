using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class SavedOpportunity
    {
        public int SavedID { get; set; }

        // Foreign Keys
        public int StudentID { get; set; }
        public int ProjectID { get; set; } // لو غيرت اسم Project لـ Opportunity غيره هنا كمان

        public DateTime SavedAt { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
