using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel
{
    // <summary>
    /// ViewModel for registering new users, including validation and switching back to login.
    /// </summary>
    public class RegisterViewModel : ObservableObject
    {
        private string _username;
        private string _password;
        private string _passwordCompare;
        private string _email;
        private string _fullname;
        private object _currentView;
        private DataService _dataService;
        private LoginViewModel _parentViewModel;

        // Validation messages for the UI, automatically updated when input changes
        public string UsernameValidationError => UsernameExists(_username) ? "Username is taken" : "";
        public string EmailValidationError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_email)) return "";
                if (!IsValidEmail(_email)) return "Invalid email";
                if (EmailExists(_email)) return "Email is already taken";
                return "";
            }
        }
        public string PasswordValidationError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_password)) return "";
                if (_password.Length < 8) return "Password must be at least 8 characters";
                if (Regex.IsMatch(_password, @"\s")) return "Password cannot contain spaces";
                if (!Regex.IsMatch(_password, @"[A-Z]")) return "Password must contain at least one uppercase letter";
                if (!Regex.IsMatch(_password, @"[a-z]")) return "Password must contain at least one lowercase letter";
                if (!Regex.IsMatch(_password, @"\d")) return "Password must contain at least one number";
                return "";
            }
        }

        public string PasswordValidationCompareError
        {
            get
            {
                if (_password != PasswordCompare) return "Password is not identical";
                return ""; 
            }
        }

        // Commands
        public ICommand CreateUserCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public object CurrentView // Updates view when changed
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
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

        public string PasswordCompare
        {
            get => _passwordCompare;
            set
            {
                if (SetProperty(ref _passwordCompare, value))
                    OnPropertyChanged(nameof(PasswordValidationCompareError));

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
        public string FullName
        {
            get => _fullname;
            set => SetProperty(ref _fullname, value);
        }
        private void ShowLogin()
        {
            _parentViewModel.ShowLoginCommand.Execute(null);
        }

        public RegisterViewModel(LoginViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;

            // Initialize commands with action and condition
            CreateUserCommand = new RelayCommand(
                _ => CreateUser(),
                _ => CanCreateUser()
                );
            ShowLoginCommand = new RelayCommand(_ => ShowLogin());
        }

        // Creates a new User and Login entry in the DataService
        private void CreateUser()
        {
            Debug.WriteLine("Creating new user...");
            int newUserId = ++_dataService.MaxUserId;
            User newUser = new User(newUserId, FullName, Email);
            Login newLogin = new Login(newUserId, Username, Password);

            Debug.WriteLine("New user created:");
            Debug.WriteLine($"ID: {newUserId}");
            Debug.WriteLine($"Name: {FullName}");
            Debug.WriteLine($"Username: {Username}");
            Debug.WriteLine($"Email: {Email}");

            _dataService.CreateUser(newUser, newLogin);
            MessageBox.Show("User successfully created!");
            // After successful registration, automatically switch back to login:
            _parentViewModel.ShowLoginCommand.Execute(null);
        }

        // Checks if all fields are valid for enabling CreateUser button
        private bool CanCreateUser()
        {
            if (string.IsNullOrWhiteSpace(FullName)) return false;
            if (string.IsNullOrWhiteSpace(Username)) return false;
            if (UsernameExists(Username)) return false;
            if (string.IsNullOrWhiteSpace(Password)) return false;
            if (EmailExists(Email)) return false;
            if (!IsValidEmail(Email)) return false;
            if (!IsValidPassword(Password)) return false;
            return true;
        }

        // Helper methods to check existing usernames/emails and validate email/password format
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
