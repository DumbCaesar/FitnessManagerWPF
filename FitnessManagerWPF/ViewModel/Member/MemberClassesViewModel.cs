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
            if (selectedClass.RegisteredMemberIds.Contains(_currentUser.Id))
            {
                selectedClass.RegisteredMemberIds.Remove(_currentUser.Id);
                Debug.WriteLine($"Removed {_currentUser.Name} from {selectedClass.Name}");
            }
            else
            {
                selectedClass.RegisteredMemberIds.Add(_currentUser.Id);
                Debug.WriteLine($"Added {_currentUser.Name} to {selectedClass.Name}");
            }
            selectedClass.IsUserEnrolled = selectedClass.RegisteredMemberIds.Contains(_currentUser.Id);
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
