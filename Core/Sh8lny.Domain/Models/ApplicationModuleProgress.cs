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
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Application Application { get; set; } = null!;
        public ProjectModule ProjectModule { get; set; } = null!;
        // Collections
        public ICollection<ApplicationModuleProgress> ModuleProgress { get; set; } = new HashSet<ApplicationModuleProgress>();
    }
}
