using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    public class Classes
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxParticipants { get; set; }
        public List<int> RegisteredMemberIds { get; set; }
    }
}
