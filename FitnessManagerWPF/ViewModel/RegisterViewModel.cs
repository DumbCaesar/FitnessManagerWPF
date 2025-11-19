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
        private void ShowLogin()
        {
            _parentViewModel.ShowLoginCommand.Execute(null);
        }

        public RegisterViewModel(LoginViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _dataService = new DataService();
            CreateUserCommand = new RelayCommand(_ => CreateUser());
            ShowLoginCommand = new RelayCommand(_ => ShowLogin());
        }
        private void CreateUser()
        {
            Debug.WriteLine($"Creating user: {Username}");
            // Implement your user creation logic here

            // After successful registration, automatically switch back to login:
            // need to use _parentViewModel.ShowLoginCommand.Execute(null) or it wont work;
        }
    }
}
