using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// A membership plan belonging to a user
    /// </summary>
    public class Purchase
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.Now;
        [JsonIgnore] public Membership Membership { get; set; }
        [JsonIgnore] public string DateDisplay => PurchasedAt.ToString("dd MMM yyyy HH:mm");
        [JsonIgnore] public string AmountDisplay => AmountPaid.ToString("C2");

        public Purchase() { }
        public Purchase(Membership membership, int id) 
        {
            Id = id;
            Membership = membership;
            MembershipId = membership.Id;
        }
    }
}
