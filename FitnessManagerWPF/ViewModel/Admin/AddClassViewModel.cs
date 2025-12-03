using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Admin
{
    class AddClassViewModel : ObservableObject
    {
        private DataService _dataService;
        private TimeSpan _time;
        private User _trainer;
        public User Trainer
        {
            get => _trainer;
            set => SetProperty(ref _trainer, value);
        }
        public string Name { get; set; }
        private string _maxParticipants = "1";
        public string MaxParticipants
        {
            get => _maxParticipants;
            set
            {
                if (string.IsNullOrEmpty(value) || 
                    (int.TryParse(value, out int num) && num >= 1 && num <= 100)) // max 100 in a class
                {
                    SetProperty(ref _maxParticipants, value);
                }
            }
        }
        public ObservableCollection<User> TrainerList { get; set; }
        public DayOfWeek Day { get; set; }
        public IEnumerable<DayOfWeek> DaysOfWeek => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public TimeSpan Time 
        {
            get => _time; 
            set => SetProperty(ref _time, value); 
        }
        public List<TimeSpan> TimeSlots
        {
            get
            {
                var times = new List<TimeSpan>();
                int startHour = 5; // gym opening time
                int endHour = 22; // gym closing time
                int intervalMinutes = 30; // time between slots

                for (int hour = startHour; hour < endHour; hour++)
                {
                    for (int minute = 0; minute < 60; minute += intervalMinutes)
                    {
                        times.Add(new TimeSpan(hour, minute, 0));
                    }
                }
                return times;
            }
        }
        public ICommand SaveClassCommand { get; set; }
        public event Action? CloseRequest;
        public ICommand CancelCommand { get; set; }
        public event Action<Classes> ClassCreated;
        public AddClassViewModel(DataService dataService) 
        {
            _dataService = dataService;
            TrainerList = new ObservableCollection<User>(_dataService.Users.Where(u => u.UserRole == UserRole.Trainer));
            SaveClassCommand = new RelayCommand(_ => SaveClass());
            CancelCommand = new RelayCommand(_ => CloseRequest?.Invoke());
        }

        private void SaveClass()
        {
            if (!int.TryParse(MaxParticipants, out int participants)) return;

            Debug.WriteLine("Creating new class...");
            int newClassId = ++_dataService.MaxClassId;
            int maxNumberOfParticipants = Convert.ToInt32(MaxParticipants);

            Debug.WriteLine($"ID: {newClassId}");
            Debug.WriteLine($"Participants: {maxNumberOfParticipants}");
            Debug.WriteLine($"Trainer: {Trainer.Name}");
            Classes newClass = new Classes
            {
                Id = newClassId,
                Name = Name,
                MaxParticipants = maxNumberOfParticipants,
                Trainer = Trainer,
                Day = Day,
                Time = Time
            };

            _dataService._activities.Add(newClass);
            _dataService.SaveClasses();
            
            ClassCreated?.Invoke(newClass);
            MessageBox.Show($"Successfully created class: {Name}");

            CloseRequest?.Invoke();
        }
    }
}
