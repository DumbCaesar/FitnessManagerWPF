using System.Diagnostics;
using System.Security;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        public event Action LoginSucceeded;

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
        }

        private void Login()
        {
            // validate login information
            Debug.WriteLine($"Username is: {Username}");
            Debug.WriteLine($"Password is: {Password}");
            LoginSucceeded?.Invoke();
        }

    }
}
