using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessManagerWPF.ViewModel;

namespace FitnessManagerWPF.Model
{
    public class Classes : ObservableObject
    {
        private bool _isUserEnrolled;
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxParticipants { get; set; }
        public ObservableCollection<int> RegisteredMemberIdsObservable { get; } = new();
        public List<int> RegisteredMemberIds
        {
            get => RegisteredMemberIdsObservable.ToList();
            set
            {
                RegisteredMemberIdsObservable.Clear();
                if (value != null)
                {
                    foreach (var id in value)
                    {
                        RegisteredMemberIdsObservable.Add(id);
                    }
                }
            }
        }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("day")]

        public DayOfWeek Day { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("time")]

        public TimeSpan Time { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string ClassSummary => $"Current Participants: {RegisteredMemberIds.Count}/{MaxParticipants}";
        [System.Text.Json.Serialization.JsonIgnore]
        public string Attendance => $"{RegisteredMemberIds.Count}/{MaxParticipants}";
        [System.Text.Json.Serialization.JsonIgnore]
        public string ClassInfo => $"{Name} - {TrainerName}";

        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsUserEnrolled 
        { 
            get => _isUserEnrolled;
            set => SetProperty(ref _isUserEnrolled, value);
        }

        public Classes()
        {
            RegisteredMemberIdsObservable.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(Attendance));
            };
        }

        public Classes(int id, string name, int participants, User trainer, DayOfWeek day, TimeSpan time)
        {
            Id = id;
            Name = name;
            MaxParticipants = participants;
            TrainerId = trainer.Id;
            TrainerName = trainer.Name;
            Day = day;
            Time = time;
        }
    }
}