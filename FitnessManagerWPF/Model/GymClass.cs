using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessManagerWPF.ViewModel;
using System.Text.Json.Serialization;

namespace FitnessManagerWPF.Model
{
    public class GymClass : ObservableObject
    {
        private bool _isUserEnrolled;
        private ObservableCollection<int> _registeredMemberIds = new();
        public int Id { get; set; } // The unique id for the Gym Class.
        public string Name { get; set; } // Class name (Yoga, HIIT).
        public int MaxParticipants { get; set; } // A maximum number of allowed participant per class.
        [JsonIgnore] public User Trainer { get; set; } // Trainer object is only used for UI Binding.
        public int TrainerId { get; set; } // for serialization
        public ObservableCollection<int> RegisteredMemberIds // List containing all of the members signed up for the class.
        {
            get => _registeredMemberIds;
            set
            {
                _registeredMemberIds = value ?? new(); // if list is empty, initialize a new one.
                // Update UI opon prop change.
                _registeredMemberIds.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CapacityDisplay)); 
                OnPropertyChanged(nameof(RegisteredMemberIds));
                OnPropertyChanged(nameof(CapacityDisplay));
            }
        }
        // Serialized day and time fields for schedueling of the class.
        [JsonPropertyName("day")] public DayOfWeek Day { get; set; }
        [JsonPropertyName("time")] public TimeSpan Time { get; set; }

        // JsonIgnore, tells Json not to save the computed properties, they are only used for binding
        [JsonIgnore] public int CurrentParticipants => RegisteredMemberIds.Count;
        [JsonIgnore] public string ClassInfo => $"{Name} - {Trainer.Name}";
        [JsonIgnore] public string CapacityDisplay => $"{CurrentParticipants}/{MaxParticipants}";

        [JsonIgnore]
        public bool IsUserEnrolled // Checks if the active user logged in is enrolled in the class.
        {
            get => _isUserEnrolled;
            set => SetProperty(ref _isUserEnrolled, value);
        }

        public GymClass() {
            // Updates relatyed computed props, whenever member list changes
            RegisteredMemberIds.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(CurrentParticipants));
                OnPropertyChanged(nameof(CapacityDisplay));
                OnPropertyChanged(nameof(IsUserEnrolled));
            };
        }
    }
}