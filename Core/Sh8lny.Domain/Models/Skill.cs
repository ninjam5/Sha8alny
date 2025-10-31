using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sh8lny.Domain.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Navigation Property
        public ICollection<StudentSkill> StudentSkills { get; set; } = new HashSet<StudentSkill>();
    }
}
