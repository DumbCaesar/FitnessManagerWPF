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
        public ObservableCollection<Classes> Classes => new ObservableCollection<Classes>(_dataService._activities);
        public ICommand SignUpCommand { get; }

        public MemberClassesViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;

            // Initialize IsUserEnrolled for each class
            foreach (Classes c in _dataService._activities)
            {
                c.IsUserEnrolled = c.RegisteredMemberIdsObservable.Contains(parentViewModel.CurrentUser.Id);
            }

            SignUpCommand = new RelayCommand(
                param => ClassSignUp(param),
                param => CanSignUp(param));
        }

        private void ClassSignUp(object? param)
        {
            if (param is Classes selectedClass)
            {
                Debug.WriteLine($"Sign up/cancel click: {selectedClass.Name}");
                User currentUser = _parentViewModel.CurrentUser;
                if (selectedClass.RegisteredMemberIdsObservable.Contains(currentUser.Id))
                {
                    selectedClass.RegisteredMemberIdsObservable.Remove(currentUser.Id);
                    selectedClass.IsUserEnrolled = false;
                    Debug.WriteLine($"Removed {currentUser.Name} from {selectedClass.Name}");
                }
                else
                {
                    selectedClass.RegisteredMemberIdsObservable.Add(currentUser.Id);
                    selectedClass.IsUserEnrolled = true;
                    Debug.WriteLine($"Added {currentUser.Name} to {selectedClass.Name}");
                }
                _dataService.SaveClasses();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool CanSignUp(object? param)
        {
            if (param is Classes selectedClass)
            {
                User currentUser = _parentViewModel.CurrentUser;

                // Only disable if class is full and currentUser is not enrolled
                if (selectedClass.RegisteredMemberIdsObservable.Count() >= selectedClass.MaxParticipants
                    && !selectedClass.RegisteredMemberIdsObservable.Contains(currentUser.Id))
                {
                    return false; // Class full so cant sign up
                }
                return true; // Either they can sign up or they are enrolled and can cancel
            }
            return false;
        }
    }
}
