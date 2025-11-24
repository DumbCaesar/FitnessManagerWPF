using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// A membership plan belonging to a user
    /// </summary>
    public class MembershipSubscription
    {
        public DateTime StartDate { get; set; } // start date for the subscription
        public DateTime EndDate { get; set; }// end date for the subscription

        public int Id { get; set; }
        public int MembershipId { get; set; }
        public MembershipSubscription() { }
        public bool isActive()
        {
            return DateTime.Now < EndDate && DateTime.Now > StartDate;
        } 
    }
}
