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
        public string MembershipType { get; set; } // string used for data binding
        public List<MembershipSubscription> BillingHistory { get; set; } // list of all subscriptions the user has had
        public DateTime DateJoined { get; set; } // Date the user signed up
        [System.Text.Json.Serialization.JsonPropertyName("role")]
        public UserRole UserRole { get; set; }
        public bool IsActiveMember => UserRole == UserRole.Member && CurrentMembership()?.IsActive == true;
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
            return BillingHistory[^1];
        }
    }

    public enum UserRole
    {
        Member,
        Trainer,
        Admin
    }
}
