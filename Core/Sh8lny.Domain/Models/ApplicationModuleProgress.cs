using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class ApplicationModuleProgress
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int ProjectModuleId { get; set; }
        
        /// <summary>
        /// Progress percentage (0-100) for this module.
        /// </summary>
        public int ProgressPercentage { get; set; }
        
        /// <summary>
        /// Optional note about the progress update.
        /// </summary>
        public string? Note { get; set; }
        
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Application Application { get; set; } = null!;
        public ProjectModule ProjectModule { get; set; } = null!;
    }
}
