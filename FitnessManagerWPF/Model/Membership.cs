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
        private float _price; 
        private int _durationMonths; // number of months the subscription lasts

        public int Id { get; set; }
        public string Name { get; set; }

        public Membership() { }
    }
}
