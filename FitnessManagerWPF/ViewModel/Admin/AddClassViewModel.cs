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
        private DayOfWeek? _day;
        private User _trainer;
       
        private const int MIN_CLASS_SIZE = 1; // min for max participants
        private const int MAX_CLASS_SIZE = 100; // max for max participants
        private const int GYM_OPENING_TIME = 5; // gym opening time
        private const int GYM_CLOSING_TIME = 22; // gym closing time
        private const int TIME_SLOT_INTERVAL = 30; // time between slots

        private string _validationError;

        public User Trainer
        {
            get => _trainer;
            set
            {
                if (SetProperty(ref _trainer, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }
        public string Name { get; set; }
        private string _maxParticipants = MIN_CLASS_SIZE.ToString();
        public string MaxParticipants
        {
            get => _maxParticipants;
            set
            {
                // Validate numeric input and ensure class capacity stays within allowed range
                if (string.IsNullOrEmpty(value) || 
                    (int.TryParse(value, out int num) && num >= MIN_CLASS_SIZE && num <= MAX_CLASS_SIZE))
                {
                    SetProperty(ref _maxParticipants, value);
                }
            }
        }
        public ObservableCollection<User> TrainerList { get; set; }
        public DayOfWeek? Day
        {
            get => _day;
            set
            {
                if (SetProperty(ref _day, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }
        public string ValidationError
        {
            get => _validationError;
            set => SetProperty(ref _validationError, value);
        }

        // Used for binding dropdown of days in UI
        public IEnumerable<DayOfWeek> DaysOfWeek => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public TimeSpan? Time 
        {
            get => _time;
            set
            {
                if (SetProperty(ref _time, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        // Generates selectable time slots (5:00 → 22:00, every 30 minutes)
        public List<TimeSpan> TimeSlots
        {
            get
            {
                var times = new List<TimeSpan>();
                for (int hour = GYM_OPENING_TIME; hour < GYM_CLOSING_TIME; hour++)
                {
                    for (int minute = 0; minute < 60; minute += TIME_SLOT_INTERVAL)
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

            if (TrainerHasConflict())
            {
                ValidationError = $"{Trainer.Name} already has a class on {Day} at {Time}";
                return false;
            }
            ValidationError = "";
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

        private bool TrainerHasConflict()
        {
            if (Trainer == null || Day == null || Time == null) return false;
            return _dataService.GymClasses.Any(c => c.TrainerId == Trainer.Id && c.Day == Day.Value && c.Time == Time.Value);
        }
    }
}
