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
        private string _email;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email
        {
            get => _email;
            set
            {
                if (value.Contains('@'))
                {
                    _email = value;
                }
            }
        }
        public string MembershipType { get; set; } // string used for data binding
        public List<MembershipSubscription> BillingHistory { get; set; } // list of all subscriptions the user has had, i.e billing history
        public DateTime DateJoined { get; set; } // Date the user signed up
        [System.Text.Json.Serialization.JsonPropertyName("role")]
        public UserRole UserRole { get; set; }

        public User() { }
        public User(int id, string name, string email, UserRole role)
        {
            Id = id;
            _email = email;
            Name = name;
            UserRole = role; 
            DateJoined = DateTime.Now;
            BillingHistory = new List<MembershipSubscription>();
        }

        public MembershipSubscription CurrentMembership()
        {
            int numberOfSubscriptions = BillingHistory?.Count ?? 0; // Certain users do not have any subscriptions
            if (numberOfSubscriptions == 0) return null;

            MembershipSubscription currentSubscription = BillingHistory[numberOfSubscriptions - 1]; // most recent subscription
            return currentSubscription;
        }
    }

    public enum UserRole
    {
        Member,
        Trainer,
        Admin
    }
}
