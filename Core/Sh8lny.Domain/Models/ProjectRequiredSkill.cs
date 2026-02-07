using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class ProjectRequiredSkill
    {
        // Primary key
        public int ProjectSkillID { get; set; }

        // Foreign keys
        public int ProjectID { get; set; }
        public int SkillID { get; set; }

        // Requirement level
        public bool IsRequired { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
