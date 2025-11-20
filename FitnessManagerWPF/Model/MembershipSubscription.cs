using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// Memberships belonging to individual members
    /// </summary>
    public class MembershipSubscription
    {
        private DateTime _startDate; // start date for the subscription
        private DateTime _endDate; // end date for the subscription

        public int Id { get; set; }
        public int MembershipId { get; set; }
        public Membership Membership { get; set; }

        public MembershipSubscription() { }
        public bool isActive()
        {
            return DateTime.Now < _endDate && DateTime.Now > _startDate;
        } 
    }
}
