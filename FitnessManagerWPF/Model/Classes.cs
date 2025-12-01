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
        [JsonIgnore] public int FreeSpots => MaxParticipants - CurrentParticipants;
        [JsonIgnore] public double OccupancyPercent => MaxParticipants > 0 ? (double)CurrentParticipants / MaxParticipants * 100 : 0;
        [JsonIgnore] public string DayTimeDisplay => $"{Day} {Time}";
        [JsonIgnore] public string CapacityDisplay => $"{CurrentParticipants}/{MaxParticipants}";
        [JsonIgnore] public string ClassInfoLine => $"{Name} • {Trainer.Name}";
        [JsonIgnore] public string ScheduleLine => $"{Day} {Time} • {Trainer.Name}";

        public Classes() { }

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
                OnPropertyChanged(nameof(FreeSpots));
                OnPropertyChanged(nameof(OccupancyPercent));
                OnPropertyChanged(nameof(CapacityDisplay));
            };
        }

        public bool IsUserEnrolled(int userId) => RegisteredMemberIds.Contains(userId);
    }
}