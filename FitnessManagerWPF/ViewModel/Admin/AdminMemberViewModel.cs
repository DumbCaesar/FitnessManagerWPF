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
    // =====================================
    //           AdminMemberViewModel
    //           Author: Nicolaj + Oliver
    // =====================================
    public class AdminMemberViewModel : ObservableObject
    {
        private object _currentView;
        private User _selectedMember;
        private AdminViewModel _parentViewModel;
        private SelectedMemberViewModel _selectedMemberViewModel;
        private ObservableCollection<User> _listOfMembers;
        private List<User> _userList;
        private DataService _dataService;
        public event Action UserCreated;

        public ICommand MemberDoubleClickCommand { get; set; }
        public ICommand NewUserCommand { get; set; }

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
            NewUserCommand = new RelayCommand(_ => ShowNewUserView());
            _parentViewModel = parentViewModel;
            _userList = new List<User>();
            _dataService = dataService;
            // Load all users and filter only members
            _userList = _dataService.Users;
            _listOfMembers = new ObservableCollection<User>(_userList.Where(u => u.UserRole == UserRole.Member));
        }

        // =====================================
        //           ShowNewUserView()
        //           Author: Oliver
        // =====================================
        private void ShowNewUserView()
        {
            // Open "Add Member" dialog
            AddMemberViewModel addMemberViewModel = new AddMemberViewModel(_dataService);
            addMemberViewModel.NewMemberCreated += OnMemberCreated;
            AddMemberView addMemberView = new AddMemberView { DataContext = addMemberViewModel };
            addMemberView.ShowDialog();
            addMemberViewModel.NewMemberCreated -= OnMemberCreated;
        }

        // =====================================
        //           OnMemberDoubleClick()
        //           Author: Oliver
        // =====================================
        private void OnMemberDoubleClick()
        {
            if (SelectedMember == null) return;
            // Open Selected Member details page
            SelectedMemberViewModel = new SelectedMemberViewModel(SelectedMember, _dataService);
            // Listen for updates/deletes from inside the window
            SelectedMemberViewModel.MemberChanged += OnMemberUpdated;
            SelectedMemberViewModel.MemberDeleted += OnMemberDeleted;
            var SelectedMemberView = new SelectedMemberView { DataContext = SelectedMemberViewModel };
            SelectedMemberView.ShowDialog();
            SelectedMemberViewModel.MemberChanged -= OnMemberUpdated;
            SelectedMemberViewModel.MemberDeleted -= OnMemberDeleted;

        }

        // =====================================
        //           OnMemberUpdated()
        //           Author: Oliver
        // =====================================
        private void OnMemberUpdated()
        {
            // Reload updated user from dataService
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

        // =====================================
        //           OnMemberDeleted()
        //           Author: Oliver
        // =====================================
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
            _parentViewModel.NotifyDataChanged();
        }

        // =====================================
        //           OnMemberCreated()
        //           Author: Oliver
        // =====================================
        private void OnMemberCreated(User user)
        {
            // Add newly created member to the list
            MemberList.Add(user);
            UserCreated?.Invoke();
            _parentViewModel.NotifyDataChanged();
        }
    }
}
