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
using System.Xml.Linq;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class EditMemberViewModel : ObservableObject
    {
        private SelectedMemberViewModel _parentViewModel;
        private DataService _dataService;
        private User _user;
        private Login _userLogin;
        private string _name;
        private string _email;
        private string _username;
        private string _password;
        public event Action MemberChanged;
        public ICommand SaveCommand { get; }
        public ICommand DiscardCommand { get; }

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
        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public Login UserLogin
        {
            get => _userLogin;
            set => SetProperty(ref _userLogin, value);
        }

        public EditMemberViewModel(SelectedMemberViewModel parentViewModel, User user, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _user = user;
            _dataService = dataService;

            SaveCommand = new RelayCommand(_ => Save());
            DiscardCommand = new RelayCommand(_ => Discard());

            // Load login info
            _userLogin = _dataService.LoadUserInfo(_user);

            // Set initial values from the user that was passed in
            Name = _user.Name ?? "";
            Email = _user.Email ?? "";
            Username = _userLogin?.Username ?? "";
            Password = _userLogin?.Password ?? "";
        }

        private void Save()
        {
            Debug.WriteLine("Save clicked");
            var messageBox = MessageBox.Show($"Are you sure you want to update {Name}'s information?", "Are you sure?", MessageBoxButton.OKCancel);
            if (messageBox == MessageBoxResult.OK)
            {
                _user.Name = Name;
                _user.Email = Email;
                _userLogin.Username = Username;
                _userLogin.Password = Password;
                _dataService.UpdateUserInfo(_user, _userLogin);
                MemberChanged?.Invoke();
                Debug.WriteLine("Saved...");
            }
            return;
        }

        private void Discard()
        {
            Debug.WriteLine("Discard clicked - reloading data");

            // Reload from data service
            var tempUser = new User { Id = _user.Id, UserRole = _user.UserRole };
            var restoredLogin = _dataService.LoadUserInfo(tempUser);

            Debug.WriteLine($"Restored Name: {tempUser.Name}, Email: {tempUser.Email}");
            Debug.WriteLine($"Restored Username: {restoredLogin?.Username}");

            // Force property updates
            Name = tempUser.Name ?? "";
            Email = tempUser.Email ?? "";
            Username = restoredLogin?.Username ?? "";
            Password = restoredLogin?.Password ?? "";

            Debug.WriteLine("Discard complete");
        }
    }
}
