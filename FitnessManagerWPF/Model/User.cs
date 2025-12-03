using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FitnessManagerWPF.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Purchase> BillingHistory { get; set; } // list of all subscriptions the user has purchased
        public DateTime DateJoined { get; set; } // Date the user signed up
        public DateTime? MembershipExpiresAt { get; set; } = null;
        public int? ActiveMembershipId { get; set; }
        [JsonIgnore] public Membership? ActiveMembership { get; set; }
        [JsonPropertyName("role")] public UserRole UserRole { get; set; }
        [JsonIgnore] public bool HasActiveMembership => DateTime.Now < MembershipExpiresAt;
        [JsonIgnore] public string MembershipStatusDisplay
        {
            get
            {
                if (MembershipExpiresAt == DateTime.MinValue || ActiveMembership == null) return "No membership";
                if (DateTime.Now >= MembershipExpiresAt) return "Expired";

                return ActiveMembership?.Name.Split(" - ").FirstOrDefault() ?? "Unknown";
            }
        }
        [JsonIgnore]
        public string MembershipExpiresDisplay
        {
            get
            {
                int daysLeft = -1;
                if (MembershipExpiresAt.HasValue)
                {
                    daysLeft = (MembershipExpiresAt.Value - DateTime.Now).Days;
                }
                return daysLeft switch
                {
                    < 0 => "",
                    0 => "Expires today",
                    1 => "1 day left",
                    _ => $"{daysLeft} days left"
                };
            }
        }

        public User() { }
        public User(int id, string name, string email, UserRole role = UserRole.Member)
        {
            Id = id;
            Email = email;
            Name = name;
            UserRole = role;
            DateJoined = DateTime.Now;
            BillingHistory = new List<Purchase>();
        }
    }

    public enum UserRole
    {
        Member,
        Trainer,
        Admin
    }
}
