using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<MembershipSubscription> BillingHistory { get; set; } // list of all subscriptions the user has had
        public DateTime DateJoined { get; set; } // Date the user signed up
        [System.Text.Json.Serialization.JsonPropertyName("role")]
        public UserRole UserRole { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] // ignore derived properties
        public string MembershipTypeDisplay
        {
            get
            {
                if (UserRole != UserRole.Member) return UserRole.ToString();
                var activeSub = CurrentMembership();
                if (activeSub != null)
                    return activeSub.Membership.Name.Split()[0]; // Remove duration from string
                return "No Active Membership";
            }
            set => MembershipTypeDisplay = value;
        }
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsActiveMember => UserRole == UserRole.Member && CurrentMembership()?.IsActive == true;
        [System.Text.Json.Serialization.JsonIgnore]
        public decimal MonthlyContribution => CurrentMembership()?.MonthlyValue ?? 0m;

        public User() { }
        public User(int id, string name, string email, UserRole role)
        {
            Id = id;
            Email = email;
            Name = name;
            UserRole = role;
            DateJoined = DateTime.Now;
            BillingHistory = new List<MembershipSubscription>();
        }

        public MembershipSubscription CurrentMembership()
        {
            if (BillingHistory == null || !BillingHistory.Any())
                return null;

            var now = DateTime.Now;

            return BillingHistory
                .Where(sub => sub.StartDate <= now && now < sub.EndDate)
                .OrderByDescending(sub => sub.StartDate)
                .FirstOrDefault();
        }
    }

    public enum UserRole
    {
        Member,
        Trainer,
        Admin
    }
}
