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
        public Membership Membership { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] // ignore derived properties
        public TimeSpan DurationLeft
        {
            get
            {
                if (EndDate <= DateTime.Now)
                    return TimeSpan.Zero;

                return EndDate - DateTime.Now;
            }
        }
        [System.Text.Json.Serialization.JsonIgnore]
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
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsActive => DateTime.Now < EndDate && DateTime.Now > StartDate;
        [System.Text.Json.Serialization.JsonIgnore]
        public decimal MonthlyValue => Membership != null && DurationLeft > TimeSpan.Zero
            ? Membership.Price / Membership.DurationInMonths
            : 0m;
        public MembershipSubscription() { }
        public MembershipSubscription(Membership membership, int id) 
        {
            Id = id;
            Membership = membership;
            MembershipId = membership.Id;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(membership.DurationInMonths);
        }
    }
}
