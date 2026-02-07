using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class StudentSkill
    {
        // Primary key
        public int StudentSkillID { get; set; }

        // Foreign keys
        public int StudentID { get; set; }
        public int SkillID { get; set; }

        // Skill proficiency
        public ProficiencyLevel? ProficiencyLevel { get; set; }

        // Timestamp
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }

    /// <summary>
    /// Proficiency level enumeration
    /// </summary>
    public enum ProficiencyLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}
