using System.Diagnostics;
using System.Security;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View;

namespace FitnessManagerWPF.ViewModel
{
    // =====================================
    //            LoginViewModel
    //      Author: Oliver og Nicolaj
    // =====================================
    /// <summary>
    /// ViewModel for handling login and navigation to role specific dashboards.
    /// </summary>
    public class LoginViewModel : ObservableObject
    {
        private readonly DataService _dataService; // Provides access to stored users and validation
        private bool _hasLoginFailed;
        private string _username;
        private string _password;
        private object _currentView; // Tracks whether login or register view is shown
        private RegisterViewModel _registerViewModel;
        public event Action LoginSucceeded; // Raised after successful login, tells the login view to close.

        public User CurrentUser { get; private set; }

        // Commands for login and switching views
        public ICommand LoginCommand { get; set; } 
        public ICommand ShowRegisterCommand { get; set; } 
        public ICommand ShowLoginCommand { get; set; } 

        public object CurrentView // The current view
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string Username // The username the user typed
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password // The password the user typed
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool HasLoginFailed
        {
            get => _hasLoginFailed;
            set => SetProperty(ref _hasLoginFailed, value);
        }

        public string ValidationError
        {
            get
            {
                if (HasLoginFailed && !_dataService.ValidateUser(Username, Password)) 
                { 
                    return "Incorrect Username or Password"; 
                }
                return "";
            }
        }

        public LoginViewModel(DataService dataService)
        {
            _dataService = dataService;
            // Bind UI buttons to actions
            LoginCommand = new RelayCommand(_ => Login());
            ShowRegisterCommand = new RelayCommand(_ => ShowRegister());
            ShowLoginCommand = new RelayCommand(_ => ShowLogin());
            _registerViewModel = new RegisterViewModel(this, _dataService);

            ShowLogin(); // Default screen
        }

        private void ShowLogin()
        {
            CurrentView = this; // Login view binds directly to this ViewModel
        }

        private void ShowRegister()
        {
            CurrentView = _registerViewModel;
        }

        private void Login()
        {
            Debug.WriteLine($"Username is: {Username}");
            Debug.WriteLine($"Password is: {Password}");

            // Attempt to validate user credentials
            if (_dataService.ValidateUser(Username, Password))
            {
                HasLoginFailed = false;
                OnPropertyChanged(nameof(ValidationError)); // Clear error
                Debug.WriteLine("Found user!");
                Debug.WriteLine($"User role is {_dataService.CurrentUser.UserRole}");
                CurrentUser = _dataService.CurrentUser;
            }
            else
            {
                Debug.WriteLine("User not found!");
                HasLoginFailed = true;
                OnPropertyChanged(nameof(ValidationError)); // Display error
                return;
            }

            if(CurrentUser != null)
            {
                // Open the correct dashboard window depending on user type
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
                // Notify listeners (close login view)
                LoginSucceeded?.Invoke();
            }
        }

    }
}
