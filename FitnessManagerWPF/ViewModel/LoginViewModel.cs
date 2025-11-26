using System.Diagnostics;
using System.Security;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View;

namespace FitnessManagerWPF.ViewModel
{
    public class LoginViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private string _username;
        private string _password;
        private object _currentView;
        private RegisterViewModel _registerViewModel;
        public event Action LoginSucceeded;

        public User CurrentUser { get; private set; }
        public ICommand LoginCommand { get; set; } // Command used for logging in
        public ICommand ShowRegisterCommand { get; set; } // responsible for switching to register user control
        public ICommand ShowLoginCommand { get; set; } // responsible for switching to login user control

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
        public LoginViewModel(DataService dataService)
        {
            _dataService = dataService;
            LoginCommand = new RelayCommand(_ => Login());
            ShowRegisterCommand = new RelayCommand(_ => ShowRegister());
            ShowLoginCommand = new RelayCommand(_ => ShowLogin());
            _registerViewModel = new RegisterViewModel(this, _dataService);

            ShowLogin();
            _username = "bob";
            _password = "password123";
        }

        private void ShowLogin()
        {
            CurrentView = this;
        }
        
        private void ShowRegister()
        {
            CurrentView = _registerViewModel;
        }

        private void Login()
        {
            Debug.WriteLine($"Username is: {Username}");
            Debug.WriteLine($"Password is: {Password}");

            if (_dataService.ValidateUser(Username, Password))
            {
                Debug.WriteLine("Found user!");
                Debug.WriteLine($"User role is {_dataService.CurrentUser.UserRole}");
                CurrentUser = _dataService.CurrentUser;
            }
            else
            {
                Debug.WriteLine("User not found!");
                return;
            }

            if(CurrentUser != null)
            {
                switch (CurrentUser.UserRole)
                {
                    case UserRole.Admin:
                        AdminViewModel adminViewModel = new AdminViewModel(CurrentUser, _dataService);
                        var adminView = new AdminView { DataContext = adminViewModel };
                        adminView.Show();
                        break;
                    case UserRole.Trainer:
                        TrainerViewModel trainerViewModel = new TrainerViewModel(CurrentUser, _dataService);
                        var trainerView = new TrainerView { DataContext = trainerViewModel };
                        trainerView.Show();
                        break;
                    case UserRole.Member:
                        MemberViewModel memberViewModel = new MemberViewModel(CurrentUser,  _dataService);
                        var memberView = new MemberView { DataContext = memberViewModel };
                        memberView.Show();
                        break;
                }
                LoginSucceeded.Invoke();
            }
        }

    }
}
