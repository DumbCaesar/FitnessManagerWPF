using System.Diagnostics;
using System.Security;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private DataService _dataService;

        public User CurrentUser { get; private set; }
        public ICommand LoginCommand { get; set; } // Command used for loggin in

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

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
            _dataService = new DataService();
            
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
            }

            switch (CurrentUser.UserRole)
            {
                // admin
                // trainer
                // member
            }
        }

    }
}
