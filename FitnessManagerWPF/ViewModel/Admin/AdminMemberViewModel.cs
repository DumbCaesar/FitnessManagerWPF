using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminMemberViewModel : ObservableObject
    {
        private object _currentView;
        private User _selectedMember;
        private AdminViewModel _parentViewModel;
        private SelectedMemberViewModel _selectedMemberViewModel;
        private ObservableCollection<User> _listOfMembers;
        private List<User> _userList;
        private DataService _dataService;

        public ICommand MemberDoubleClickCommand { get; set; }

        public SelectedMemberViewModel SelectedMemberViewModel
        {
            get => _selectedMemberViewModel;
            set => SetProperty(ref _selectedMemberViewModel, value);
        }

        public ObservableCollection<User> MemberList
        {
            get => _listOfMembers;
            set => SetProperty(ref _listOfMembers, value);
        }

        public User SelectedMember
        {
            get => _selectedMember;
            set 
            {
                SetProperty(ref _selectedMember, value);
                if(SelectedMember != null)
                {
                    Debug.WriteLine($"Selected Member is: {_selectedMember.Name}");
                }
                

            }
        } 

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminMemberViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            MemberDoubleClickCommand = new RelayCommand(_ => OnMemberDoubleClick());
            _parentViewModel = parentViewModel;
            _userList = new List<User>();
            _dataService = dataService;
            _userList = _dataService.Users;
            _listOfMembers = new ObservableCollection<User>(_userList.Where(u => u.UserRole == UserRole.Member));
        }

        private void OnMemberDoubleClick()
        {
            if (SelectedMember == null) return;
            SelectedMemberViewModel = new SelectedMemberViewModel(SelectedMember, _dataService);
            SelectedMemberViewModel.MemberChanged += OnMemberUpdated;
            SelectedMemberViewModel.MemberDeleted += OnMemberDeleted;
            var SelectedMemberView = new SelectedMemberView { DataContext = SelectedMemberViewModel };
            SelectedMemberView.ShowDialog();
            SelectedMemberViewModel.MemberChanged -= OnMemberUpdated;
            SelectedMemberViewModel.MemberDeleted -= OnMemberDeleted;

        }

        private void OnMemberUpdated()
        {
            _userList = _dataService.Users;
            var updatedUser = _userList.FirstOrDefault(u => u.Id == SelectedMember.Id);

            if (updatedUser != null)
            {
                var index = MemberList.ToList().FindIndex(m => m.Id == SelectedMember.Id);
                if (index >= 0)
                {
                    // Remove and insert to force collection change notification
                    var oldUser = MemberList[index];
                    MemberList.RemoveAt(index);
                    MemberList.Insert(index, updatedUser);

                    Debug.WriteLine($"Removed: {oldUser.Name}, Inserted: {updatedUser.Name}");
                    Debug.WriteLine($"Are they the same object? {ReferenceEquals(oldUser, updatedUser)}");
                }
            }
        }

        private void OnMemberDeleted()
        {
            // SelectedUser was deleted, remove from ObservableCollection
            var userToRemove = MemberList.FirstOrDefault(m => m.Id == SelectedMember.Id);
            if (userToRemove != null)
            {
                MemberList.Remove(userToRemove);
                Debug.WriteLine($"Removed deleted user: {userToRemove.Name} (ID: {userToRemove.Id})");
            }
            // Clear the selection since the user no longer exists
            SelectedMember = null;
        }


    }
}
