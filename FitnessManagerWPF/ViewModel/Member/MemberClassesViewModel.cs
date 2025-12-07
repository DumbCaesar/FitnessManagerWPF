using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Member
{
    /// <summary>
    /// ViewModel for managing class sign-ups for a member.
    /// </summary>
    public class MemberClassesViewModel
    {
        private DataService _dataService; // Access to stored gym classes
        private MemberViewModel _parentViewModel;
        private User _currentUser; // The member currently using the system
        public ObservableCollection<GymClass> Classes { get; set; } // Classes displayed in UI
        public ICommand SignUpCommand { get; } // Sign up/cancel command for each class

        public MemberClassesViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            _currentUser = parentViewModel.CurrentUser;

            Classes = new ObservableCollection<GymClass>(_dataService.GymClasses);

            // Initialize command with execution and can-execute logic
            SignUpCommand = new RelayCommand(
                param => ClassSignUp(param),
                param => CanSignUp(param));
        }

        // Handles sign-up or cancellation for a gym class
        private void ClassSignUp(object? param)
        {
            if (param is not GymClass selectedClass) return;

            Debug.WriteLine($"Sign up/cancel click: {selectedClass.Name}");
            if (selectedClass.RegisteredMemberIds.Contains(_currentUser.Id)) 
            {
                selectedClass.RegisteredMemberIds.Remove(_currentUser.Id); // Cancel sign-up
                Debug.WriteLine($"Removed {_currentUser.Name} from {selectedClass.Name}");
            }
            else
            {
                selectedClass.RegisteredMemberIds.Add(_currentUser.Id); // Sign up
                Debug.WriteLine($"Added {_currentUser.Name} to {selectedClass.Name}");
            }
            selectedClass.IsUserEnrolled = selectedClass.RegisteredMemberIds.Contains(_currentUser.Id);
            _dataService.SaveGymClasses();
            CommandManager.InvalidateRequerySuggested();
        }

        // Determines if sign-up/cancel button should be enabled
        private bool CanSignUp(object? param)
        {
            if (param is not GymClass selectedClass) return false;
            if (!_currentUser.HasActiveMembership) return false;
            // Only disable if class is full and currentUser is not enrolled
            bool isEnrolled = selectedClass.RegisteredMemberIds.Contains(_currentUser.Id);
            bool isFull = selectedClass.CurrentParticipants >= selectedClass.MaxParticipants;

            return isEnrolled || !isFull; // Allow if enrolled or class not full
        }
    }
}
