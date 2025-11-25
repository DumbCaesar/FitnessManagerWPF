using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _email;
        private string _fullname;
        private object _currentView;
        private DataService _dataService;
        private LoginViewModel _parentViewModel;

        public ICommand CreateUserCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
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
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
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
            CreateUserCommand = new RelayCommand(
                _ => CreateUser(),
                _ => CanCreateUser()
                );
            ShowLoginCommand = new RelayCommand(_ => ShowLogin());
        }
        private void CreateUser()
        {
            Debug.WriteLine("Creating new user...");
            int newUserId = ++_dataService.MaxUserId;
            User newUser = new User(newUserId, FullName, Email, UserRole.Member);
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

        private bool CanCreateUser()
        {
            if (string.IsNullOrWhiteSpace(FullName)) return false;
            if (string.IsNullOrWhiteSpace(Username)) return false;
            if (UsernameExists(Username)) return false;
            if (string.IsNullOrWhiteSpace(Password)) return false;
            if (EmailExists(Email)) return false;
            if (!IsValidEmail(Email)) return false;
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
    }
}
