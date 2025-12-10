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
    // =====================================
    //          AddClassViewModel
    //      Author: Oliver + Nicolaj
    // =====================================
    /// <summary>
    /// ViewModel used for creating a new gym class inside the Admin panel.
    /// Handles trainer selection, schedule, participant limits, and class creation.
    /// </summary>
    public class AddClassViewModel : ObservableObject
    {
        private DataService _dataService;
        private TimeSpan? _time;
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
                // Validate numeric input and ensure class capacity stays within allowed range
                if (string.IsNullOrEmpty(value) || 
                    (int.TryParse(value, out int num) && num >= 1 && num <= 100)) // max 100 in a class
                {
                    SetProperty(ref _maxParticipants, value);
                }
            }
        }
        public ObservableCollection<User> TrainerList { get; set; }
        public DayOfWeek? Day { get; set; }

        // Used for binding dropdown of days in UI
        public IEnumerable<DayOfWeek> DaysOfWeek => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public TimeSpan? Time 
        {
            get => _time; 
            set => SetProperty(ref _time, value); 
        }

        // Generates selectable time slots (5:00 → 22:00, every 30 minutes)
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

        // Event for closing the Add Class dialog
        public event Action CloseRequest;
        public ICommand CancelCommand { get; set; }

        // Notifies parent UI that a new class was created
        public event Action<GymClass> ClassCreated;
        public AddClassViewModel(DataService dataService) 
        {
            _dataService = dataService;
            // Populate trainer list with only users who are trainers
            TrainerList = new ObservableCollection<User>(_dataService.Users.Where(u => u.UserRole == UserRole.Trainer));
            SaveClassCommand = new RelayCommand(
                _ => SaveClass(),
                _ => CanSaveClass());
            CancelCommand = new RelayCommand(_ => CloseRequest?.Invoke());
        }

        // =====================================
        //            CanSaveClass()
        //           Author: Oliver
        // =====================================
        // Validates if the input in add class is valid.
        private bool CanSaveClass()
        {
            if (Name == null || Name.Length < 4) return false;
            if (MaxParticipants == null) return false;
            if (Trainer == null) return false;
            if (Day == null) return false;
            if (Time == null) return false;
            return true;
        }

        // =====================================
        //             SaveClass()
        //           Author: Oliver
        // =====================================
        private void SaveClass()
        {
            // If participant count is invalid, abort
            if (!int.TryParse(MaxParticipants, out int participants)) return;

            Debug.WriteLine("Creating new class...");
            int newClassId = ++_dataService.MaxClassId;
            int maxNumberOfParticipants = Convert.ToInt32(MaxParticipants);

            Debug.WriteLine($"ID: {newClassId}");
            Debug.WriteLine($"Participants: {maxNumberOfParticipants}");
            Debug.WriteLine($"Trainer: {Trainer.Name}");
            GymClass newClass = new GymClass
            {
                Id = newClassId,
                Name = Name,
                MaxParticipants = maxNumberOfParticipants,
                Trainer = Trainer,
                Day = Day.Value,
                Time = Time.Value
            };

            _dataService.GymClasses.Add(newClass);
            _dataService.SaveGymClasses();

            // Notify UI layer and close window, pass along the new class as well.
            ClassCreated?.Invoke(newClass);
            MessageBox.Show($"Successfully created class: {Name}");
            CloseRequest?.Invoke();
        }
    }
}
