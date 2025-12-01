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
    public class MemberClassesViewModel
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;
        private User _currentUser;
        public ObservableCollection<Classes> Classes { get; set; }
        public ICommand SignUpCommand { get; }

        public MemberClassesViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            _currentUser = parentViewModel.CurrentUser;

            Classes = new ObservableCollection<Classes>(_dataService._activities);

            SignUpCommand = new RelayCommand(
                param => ClassSignUp(param),
                param => CanSignUp(param));
        }

        private void ClassSignUp(object? param)
        {
            if (param is not Classes selectedClass) return;

            Debug.WriteLine($"Sign up/cancel click: {selectedClass.Name}");
            User currentUser = _parentViewModel.CurrentUser;
            if (selectedClass.RegisteredMemberIds.Contains(currentUser.Id))
            {
                selectedClass.RegisteredMemberIds.Remove(currentUser.Id);
                Debug.WriteLine($"Removed {currentUser.Name} from {selectedClass.Name}");
            }
            else
            {
                selectedClass.RegisteredMemberIds.Add(currentUser.Id);
                Debug.WriteLine($"Added {currentUser.Name} to {selectedClass.Name}");
            }
            _dataService.SaveClasses();
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanSignUp(object? param)
        {
            if (param is not Classes selectedClass) return false;
            // Only disable if class is full and currentUser is not enrolled
            bool isEnrolled = selectedClass.RegisteredMemberIds.Contains(_currentUser.Id);
            bool isFull = selectedClass.CurrentParticipants >= selectedClass.MaxParticipants;

            return isEnrolled || !isFull;
        }
    }
}
