using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.Model
{
    public class Classes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxParticipants { get; set; }
        public List<int> RegisteredMemberIds { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("day")]

        public DayOfWeek Day { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("time")]

        public TimeSpan Time { get; set; }

        public string ClassSummary => $"Current Participants: {RegisteredMemberIds.Count}/{MaxParticipants}";

        public string ClassInfo => $"{Name} - {TrainerName}";
    }
}
