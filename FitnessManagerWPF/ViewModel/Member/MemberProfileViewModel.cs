using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

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
            set
            {
                if (SetProperty(ref _name, value))
                    OnPropertyChanged(nameof(NameValidationError));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    OnPropertyChanged(nameof(EmailValidationError));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                    OnPropertyChanged(nameof(UsernameValidationError));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    OnPropertyChanged(nameof(PasswordValidationError));
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                    OnPropertyChanged(nameof(PasswordValidationError));
            }
        }

        public string NewPasswordCompare
        {
            get => _newPasswordCompare;
            set
            {
                if (SetProperty(ref _newPasswordCompare, value))
                    OnPropertyChanged(nameof(PasswordValidationError));
            }
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
        public string UsernameValidationError
        {
            get
            {
                if (string.IsNullOrEmpty(Username)) return "Username cannot be empty";
                return Username != _currentUserLogin.Username && UsernameExists(Username) ? "Username is taken" : "";
            }
        }
        public string EmailValidationError
        {
            get
            {
                if (!IsValidEmail(_email)) return "Invalid email";
                if (Email != _currentUser.Email && EmailExists(Email)) return "Email is already taken";
                return "";
            }
        }
        public string PasswordValidationError
        {
            get
            {
                if (string.IsNullOrEmpty(Password)) return "Enter password to save changes";
                if (Password != _currentUserLogin.Password) return "Incorrect password";
                if (!string.IsNullOrEmpty(_newPassword) || !string.IsNullOrEmpty(_newPasswordCompare))
                {
                    if (!IsValidPassword(NewPassword)) return "Password must contain 8+ characters, uppercase, lowercase and a digit";
                    if (NewPassword != NewPasswordCompare) return "Passwords do not match";
                }
                return "";
            }
        }
        public string NameValidationError => string.IsNullOrEmpty(Name) ? "Name cannot be empty" : "";

        public MemberProfileViewModel(MemberViewModel parentViewModel, DataService dataService, User user)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            _currentUser = user;
            _dateJoined = user.DateJoined;
            _parentViewModel.DataChanged += UpdateBillingHistory;
            SaveCommand = new RelayCommand(_ => Save(),
                                           _ => CanSaveChanges());
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

        private bool CanSaveChanges()
        {
            if (string.IsNullOrEmpty(Password)) return false;
            if (Password != _currentUserLogin.Password) return false; // validate password
            
            // required fields
            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (string.IsNullOrWhiteSpace(Username)) return false;
            if (!IsValidEmail(Email)) return false;

            // unique username
            if (Username != _currentUserLogin.Username && UsernameExists(Username)) return false;

            // unique email
            if (Email != _currentUser.Email && EmailExists(Email)) return false;

            // optional newpassword
            bool newPasswordEntered = !string.IsNullOrEmpty(NewPassword);
            bool comparePasswordEntered = !string.IsNullOrEmpty(NewPasswordCompare);

            if (newPasswordEntered || comparePasswordEntered)
            {
                if (!newPasswordEntered || !comparePasswordEntered) return false;
                if (!IsValidPassword(NewPassword)) return false;
                if (NewPassword != NewPasswordCompare) return false;
            }
            return true;
        }

        private bool UsernameExists(string username) => _dataService.Logins.Any(u => u.Username == username);
        private bool EmailExists(string email) => _dataService.Users.Any(u => u.Email == email);
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            if (!MailAddress.TryCreate(email, out var mailAddress)) return false;
            return mailAddress.Host.Contains('.');
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < 8) return false;

            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasDigit = Regex.IsMatch(password, @"\d");

            return hasUpperCase && hasLowerCase && hasDigit;
        }
    }
}