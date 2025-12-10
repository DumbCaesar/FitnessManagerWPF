using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FitnessManagerWPF.Model
{
    // =====================================
    //              Purchase
    //     Author: Oliver og Nicolaj
    // =====================================
    public class Purchase
    {
        public int Id { get; set; } // an auto incrementing id for every purchase.
        public int MembershipId { get; set; } // The id of the selected membership plan
        public decimal AmountPaid { get; set; } // The price paid.
        public DateTime PurchasedAt { get; set; } = DateTime.Now; // Get's the exact time a purchase was made.

        // Reference to actual membership (not serialized)
        [JsonIgnore] public Membership Membership { get; set; }
        // Formatted date for UI display ("12 Jan 2025 14:20")
        [JsonIgnore] public string DateDisplay => PurchasedAt.ToString("dd MMM yyyy HH:mm");
        // Formatted price with currency symbol
        [JsonIgnore] public string AmountDisplay => AmountPaid.ToString("C2");

        public Purchase() { }
    }
}
