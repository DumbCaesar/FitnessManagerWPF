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
        public decimal Price { get; set; } // The price of said membership
        public int DurationInMonths { get; set; } // number of months the subscription lasts

        public int Id { get; set; } // unique id, for each membership package 1-6 currently.
        public string Name { get; set; } // The membership package (basic, VIP..)

        public Membership() { }
    }
}
