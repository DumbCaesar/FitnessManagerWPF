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
        public DateTime StartDate { get; set; } // start date for the subscription
        public DateTime EndDate { get; set; }// end date for the subscription
        public TimeSpan DurationLeft
        {
            get
            {
                if (EndDate <= DateTime.Now)
                    return TimeSpan.Zero;

                return EndDate - DateTime.Now;
            }
        }
        public string DurationLeftFormatted
        {
            get
            {
                var span = DurationLeft;

                if (span == TimeSpan.Zero)
                    return "Expired";

                return $"{span.Days} days, {span.Hours} hours, {span.Minutes} minutes";
            }
        }
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public MembershipSubscription() { }
        public bool isActive()
        {
            return DateTime.Now < EndDate && DateTime.Now > StartDate;
        } 
    }
}
