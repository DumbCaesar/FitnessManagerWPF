using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    // =====================================
    //         AddMemberViewModel
    //       Author: Oliver + Nicolaj
    // =====================================
    public class AddMemberViewModel : ObservableObject
    {
        private string _name;
        private string _email;
        private string _username;
        private string _password;
        private DataService _dataService;
        // Events for notifying parent window that a new member has been created
        // and for closing the window.
        public event Action<User> NewMemberCreated;
        public event Action CloseView;

        public ICommand CancelCommand { get; set; }
        public ICommand CreateUserCommand { get; set; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
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

        public AddMemberViewModel(DataService dataService)
        {
            _dataService = dataService;
            CancelCommand = new RelayCommand(_ => Cancel());
            CreateUserCommand = new RelayCommand(_ => CreateUser(),
                                                 _ => CanCreateUser());
        }

        // =====================================
        //              Cancel()
        //           Author: Oliver
        // =====================================
        private void Cancel()
        {
            // Close this window
            CloseView?.Invoke();
        }

        // =====================================
        //              CreateUser()
        //           Author: Oliver
        // =====================================
        private void CreateUser()
        {
            int userId = ++_dataService.MaxUserId;
            User user = new User(userId, Name, Email);
            Login login = new Login(userId, Username, Password);
            Debug.WriteLine("New user created:");
            Debug.WriteLine($"ID: {userId}");
            Debug.WriteLine($"Name: {Name}");
            Debug.WriteLine($"Username: {Username}");
            Debug.WriteLine($"Email: {Email}");
            _dataService.CreateUser(user, login);
            MessageBox.Show("User successfully created!");
            // Notify UI and close window
            NewMemberCreated?.Invoke(user);
            CloseView?.Invoke();
        }

        private bool CanCreateUser()
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;
            // check if user is taken
            var existingLogin = _dataService.Logins.FirstOrDefault(l => l.Username == Username);
            // check if email is taken
            var existingUser = _dataService.Users.FirstOrDefault(u => u.Email == Email);
            return existingLogin == null && existingUser == null;
        }
    }
}
