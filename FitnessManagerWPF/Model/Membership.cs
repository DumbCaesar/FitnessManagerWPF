using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// A membership plan offered by the gym
    /// </summary>
    public class Membership
    {
        public decimal Price { get; set; }
        public int DurationMonths { get; set; } // number of months the subscription lasts

        public int Id { get; set; }
        public string Name { get; set; }

        public Membership() { }
    }
}
