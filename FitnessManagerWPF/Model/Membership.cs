using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// Membership plans offered by the gym
    /// </summary>
    public class Membership
    {
        public float Price { get; set; }
        public int DurationMonths { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }

        public Membership() { }
    }
}
