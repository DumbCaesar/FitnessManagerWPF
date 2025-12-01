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
    public class Classes : ObservableObject
    {
        private bool _isUserEnrolled;
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxParticipants { get; set; }
        [JsonIgnore] public User Trainer { get; set; } // For display and binding
        public int TrainerId { get; set; } // for serialization
        public ObservableCollection<int> RegisteredMemberIds { get; } = new();
        [JsonPropertyName("day")] public DayOfWeek Day { get; set; }
        [JsonPropertyName("time")] public TimeSpan Time { get; set; }

        // Ignore the computed properties used for binding
        [JsonIgnore] public int CurrentParticipants => RegisteredMemberIds.Count;
        [JsonIgnore] public string CapacityDisplay => $"{CurrentParticipants}/{MaxParticipants}";

        [JsonIgnore]
        public bool IsUserEnrolled
        {
            get => _isUserEnrolled;
            set => SetProperty(ref _isUserEnrolled, value);
        }

        public Classes() {
            RegisteredMemberIds.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(CurrentParticipants));
                OnPropertyChanged(nameof(CapacityDisplay));
                OnPropertyChanged(nameof(IsUserEnrolled));
            };
        }

        public Classes(int id, string name, int participants, User trainer, DayOfWeek day, TimeSpan time)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(trainer));
            MaxParticipants = participants;
            Trainer = trainer ?? throw new ArgumentNullException(nameof(trainer));
            TrainerId = trainer.Id;
            Day = day;
            Time = time;

            RegisteredMemberIds.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(CurrentParticipants));
                OnPropertyChanged(nameof(CapacityDisplay));
                OnPropertyChanged(nameof(IsUserEnrolled));
            };
        }
    }
}