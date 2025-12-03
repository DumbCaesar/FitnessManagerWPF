using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberProfileViewModel : ObservableObject
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;
        private MemberMembershipViewModel _memberMembershipViewModel;
        private User _currentUser;
        private Login _currentUserLogin;
        private string _name;
        private string _email;
        private string _username;
        private string _password;
        private string _newPassword;
        private string _newPasswordCompare;
        private DateTime _dateJoined;
        private int _membershipId;

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

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string NewPasswordCompare
        {
            get => _newPasswordCompare;
            set => SetProperty(ref _newPasswordCompare, value);
        }

        public DateTime DateJoined
        {
            get => _dateJoined;
        }

        public string MembershipTypeDisplay
        {
            get => _currentUser?.MembershipStatusDisplay ?? "No Active Membership";
        }

        public int MembershipId
        {
            get => _membershipId;
            set => SetProperty(ref _membershipId, value);
        }


        public MemberProfileViewModel(MemberViewModel parentViewModel, DataService dataService, User user)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            _currentUser = user;
            _dateJoined = user.DateJoined;

            SaveCommand = new RelayCommand(_ => Save());
            DiscardCommand = new RelayCommand(_ => Discard());

            // Load login info
            _currentUserLogin = _dataService.LoadUserInfo(_currentUser);

            // Set initial values from the user that was passed in
            Name = _currentUser.Name ?? "";
            Email = _currentUser.Email ?? "";
            Username = _currentUserLogin?.Username ?? "";
        }

        private void Save()
        {
            Debug.WriteLine("Save clicked");
            var messageBox = MessageBox.Show("Are you sure you want to update your information?", "Are you sure?", MessageBoxButton.OKCancel);
            if (messageBox == MessageBoxResult.OK)
            {
                if(Password == _currentUserLogin.Password && string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(NewPasswordCompare))
                {
                    _currentUser.Name = Name;
                    _currentUser.Email = Email;
                    _currentUserLogin.Username = Username;
                    _currentUserLogin.Password = Password;
                    _currentUserLogin.MembershipId = MembershipId;
                    _dataService.UpdateUserInfo(_currentUser, _currentUserLogin);
                    Debug.WriteLine("Saved...");
                }
                if (string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(NewPasswordCompare)) return;
                if(Password == _currentUserLogin.Password && NewPassword == NewPasswordCompare)
                {
                    _currentUser.Name = Name;
                    _currentUser.Email = Email;
                    _currentUserLogin.Username = Username;
                    _currentUserLogin.Password = NewPassword;
                    _currentUserLogin.MembershipId = MembershipId;
                    _dataService.UpdateUserInfo(_currentUser, _currentUserLogin);
                    Debug.WriteLine("Saved...");
                }
                
            }
            return;
        }

        private void Discard()
        {
            Debug.WriteLine("Discard clicked - reloading data");

            // Reload from data service
            var tempUser = new User { Id = _currentUser.Id, UserRole = _currentUser.UserRole };
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

        public void UpdateMemberRole()
        {
            OnPropertyChanged(nameof(MembershipTypeDisplay));
        }
    }
}