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

                return $"{span.Days} days, {span.Hours} hours, {span.Minutes} minutes left";
            }
        }
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public Membership Membership { get; set; }
        public bool IsActive => DateTime.Now < EndDate && DateTime.Now > StartDate;
        public decimal MonthlyValue => Membership != null && Membership.DurationMonths > 0
            ? Membership.Price / Membership.DurationMonths
            : 0m;
        public MembershipSubscription() { }
    }
}
