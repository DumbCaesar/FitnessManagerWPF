using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FitnessManagerWPF.Model
{
    // =====================================
    //               User
    //     Author: Oliver og Nicolaj
    // =====================================
    public class User
    {
        public int Id { get; set; } // Unique user id
        public string Name { get; set; }
        public string Email { get; set; } 
        public List<Purchase> BillingHistory { get; set; } // list of all subscriptions the user has purchased
        public DateTime DateJoined { get; set; } // Date the user signed up
        // Date when the current membership ends (if null, no membership)
        public DateTime? MembershipExpiresAt { get; set; } = null;
        public int? ActiveMembershipId { get; set; } // Get's the current active id of the membership
        [JsonIgnore] public Membership? ActiveMembership { get; set; } // Reference to membership used for UI binding.
        [JsonPropertyName("role")] public UserRole UserRole { get; set; } // Role of the user (Member, Trainer or Admin)
        [JsonIgnore] public bool HasActiveMembership => DateTime.Now < MembershipExpiresAt; // Whether the membership is currently valid
        [JsonIgnore] public string MembershipStatusDisplay // Provides a short description of the membership status
        {
            get
            {
                // If user never had a membership or it's missing data
                if (MembershipExpiresAt == DateTime.MinValue || ActiveMembership == null) return "No membership";
                // Membership expired
                if (DateTime.Now >= MembershipExpiresAt) return "Expired";
                // Show membership name without duration part
                return ActiveMembership?.Name.Split(" - ").FirstOrDefault() ?? "Unknown";
            }
        }
        [JsonIgnore]
        public string MembershipExpiresDisplay // Displays how long until the membership expires (e.g., "2 days left")
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
                    < 0 => "", // Already expired or no membership
                    0 => "Expires today",
                    1 => "1 day left",
                    _ => $"{daysLeft} days left"
                };
            }
        }

        public User() { }
        public User(int id, string name, string email, UserRole role = UserRole.Member) // Constructor for creating a user
        {
            Id = id;
            Email = email;
            Name = name;
            UserRole = role;
            DateJoined = DateTime.Now;
            BillingHistory = new List<Purchase>();
        }
    }

    public enum UserRole // Roles
    {
        Member,
        Trainer,
        Admin
    }
}
