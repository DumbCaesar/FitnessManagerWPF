using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using FitnessManagerWPF.ViewModel;

namespace FitnessManagerWPF.Model
{
    /// <summary>
    /// A membership plan offered by the gym
    /// </summary>
    public class Membership : ObservableObject
    {
        [JsonIgnore] private bool _isActiveType; // if membership has same type as user's current active membership
        public decimal Price { get; set; } // The price of said membership
        public int DurationInMonths { get; set; } // number of months the subscription lasts

        public int Id { get; set; } // unique id, for each membership package 1-6 currently.
        public string Name { get; set; } // The membership package (basic, VIP..)
        public bool IsActiveType
        {
            get => _isActiveType;
            set => SetProperty(ref _isActiveType, value);
        }
        [JsonIgnore] public string AmountDisplay => Price.ToString("C2");

        public Membership() { }

    }
}
