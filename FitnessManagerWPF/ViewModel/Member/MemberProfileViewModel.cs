using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Collections.ObjectModel;

namespace FitnessManagerWPF.ViewModel.Member
{
    /// <summary>
    /// ViewModel for managing member profile information, including personal details and password updates.
    /// </summary>
    public class MemberProfileViewModel : ObservableObject
    {
        private DataService _dataService; // Access to users and logins
        private MemberViewModel _parentViewModel;
        private User _currentUser; // Logged-in user
        private Login _currentUserLogin; // Login credentials for the user

        // Backing fields for UI-bound properties
        private string _name;
        private string _email;
        private string _username;
        private string _password;
        private string _newPassword;
        private string _newPasswordCompare;
        private DateTime _dateJoined;
        private int _membershipId;
        private string _membershipExpiresDisplay;
        private ObservableCollection<Purchase> _userSubscriptions; // User's purchase history

        // Commands bound to Save and Discard buttons
        public ICommand SaveCommand { get; }
        public ICommand DiscardCommand { get; }

        // Properties bound to UI fields
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string NewPasswordCompare
        {
            get => _newPasswordCompare;
            set => SetProperty(ref _newPasswordCompare, value);
        }

        public DateTime DateJoined
        {
            get => _dateJoined;
        }

        public string MembershipTypeDisplay
        {
            get => _currentUser?.MembershipStatusDisplay ?? "No Active Membership";
        }

        public string MembershipExpiresDisplay
        {
            get => _currentUser?.MembershipExpiresDisplay ?? "";
            set => SetProperty(ref _membershipExpiresDisplay, value);
        }

        public int MembershipId
        {
            get => _membershipId;
            set => SetProperty(ref _membershipId, value);
        }

        public ObservableCollection<Purchase> UserSubscriptions
        {
            get => _userSubscriptions;
            set => SetProperty(ref _userSubscriptions, value);
        }


        public MemberProfileViewModel(MemberViewModel parentViewModel, DataService dataService, User user)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            _currentUser = user;
            _dateJoined = user.DateJoined;
            _parentViewModel.DataChanged += UpdateBillingHistory;
            SaveCommand = new RelayCommand(_ => Save());
            DiscardCommand = new RelayCommand(_ => Discard());

            // Load login info
            _currentUserLogin = _dataService.LoadUserInfo(_currentUser);

            // Set initial values from the user that was passed in
            Name = _currentUser.Name ?? "";
            Email = _currentUser.Email ?? "";
            Username = _currentUserLogin?.Username ?? "";
            UserSubscriptions = new ObservableCollection<Purchase>(user.BillingHistory);
        }

        private void Save()
        {
            var messageBox = MessageBox.Show("Are you sure you want to update your information?", "Are you sure?", MessageBoxButton.OKCancel);
            if (messageBox != MessageBoxResult.OK) return;

            if (Password != _currentUserLogin.Password) return; // validate password

            if (!string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(NewPasswordCompare))
            {
                if (NewPassword != NewPasswordCompare) return; // validate new password
                _currentUserLogin.Password = NewPassword;
            }
            Debug.WriteLine($"Username before: {_currentUserLogin.Username}");
            _currentUser.Name = Name;
            _currentUser.Email = Email;
            _currentUserLogin.Username = Username;
            Debug.WriteLine($"Username after: {_currentUserLogin.Username}");


            _dataService.SaveUsers();
            _dataService.SaveLogins();

            // Clear password fields after save
            Password = "";
            NewPassword = "";
            NewPasswordCompare = "";

            Debug.WriteLine("Saved...");
        }

        private void Discard()
        {
            Debug.WriteLine("Discard clicked - reloading data");
            // Force property updates
            Name = _currentUser.Name ?? "";
            Email = _currentUser.Email ?? "";
            Username = _currentUserLogin.Username ?? "";

            // Clear password fields after discard
            Password = "";
            NewPassword = "";
            NewPasswordCompare = "";
            Debug.WriteLine($"Restored Name: {_currentUser.Name}, Email: {_currentUser.Email}");
            Debug.WriteLine($"Restored Username: {_currentUserLogin?.Username}");
        }

        public void UpdateMemberRole() 
        {
            OnPropertyChanged(nameof(MembershipTypeDisplay));
        }

        private void UpdateBillingHistory()
        {
            UserSubscriptions = new ObservableCollection<Purchase>(_currentUser.BillingHistory);
        }
    }
}