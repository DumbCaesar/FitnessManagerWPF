using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            CreateUserCommand = new RelayCommand(_ => CreateUser());
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
            Debug.WriteLine($"Password: {Password}");
            // After successful registration, automatically switch back to login:
            // need to use _parentViewModel.ShowLoginCommand.Execute(null) or it wont work;
        }
    }
}
