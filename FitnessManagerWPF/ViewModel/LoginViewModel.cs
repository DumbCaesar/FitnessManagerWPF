using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        public event Action LoginSucceeded;

        public ICommand LoginCommand { get; set; } // Command used for loggin in

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public SecureString Password
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
