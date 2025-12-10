using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View;
using FitnessManagerWPF.View.UserControl.Admin;
using FitnessManagerWPF.ViewModel.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel
{
    /// <summary>
    /// Handles navigation and state for the Admin section of the application.
    /// </summary>
    public class AdminViewModel : ObservableObject
    {
        private readonly DataService _dataService; // Shared data acess service
        private User _currentUser; // the logged in user (admin)
        private object _currentView; // used for changing the userControl to switch views.
        // Sub-viewmodels for each admin page
        private AdminDashboardViewModel _adminDashboardViewModel;
        private AdminClassesViewModel _adminClassesViewModel;
        private AdminMemberViewModel _adminMemberViewModel;
        private AdminTrainerViewModel _adminTrainerViewModel;
        public event Action DataChanged; // Event for updating UI when underlying data updates.
        public event Action Logout; // Event invoked when logging out, closes AdminView.

        // Commands for switching between admin pages
        public ICommand DashboardCommand { get; set; }
        public ICommand MemberCommand { get; set; }
        public ICommand TrainerCommand { get; set; } 
        public ICommand ClassesCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        // The active view shown in the admin panel
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // The admin user currently logged in
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty (ref _currentUser, value);
        }
        public AdminViewModel(User user, DataService dataService)
        {
            _currentUser = user;
            _dataService = dataService;
            // Initialize viewmodels
            _adminClassesViewModel = new AdminClassesViewModel(this, _dataService);
            _adminDashboardViewModel = new AdminDashboardViewModel(this, _dataService);
            _adminMemberViewModel = new AdminMemberViewModel(this, _dataService);
            _adminTrainerViewModel = new AdminTrainerViewModel(this, _dataService);
            // Setup navigation commands
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            MemberCommand = new RelayCommand(_ => ShowMembers());
            TrainerCommand = new RelayCommand(_ => ShowTrainers());
            ClassesCommand = new RelayCommand(_ => ShowClasses());
            LogoutCommand = new RelayCommand(_ => OnLogout());
            // Default page (start page)
            ShowDashboard();
        }

        public AdminViewModel() // Empty constructor used for DataContext
        {
            
        }

        private void OnLogout()
        {
            var loginView = new LoginView();
            loginView.Show();
            Logout?.Invoke();
        }

        // The command methods for switching to a different view.
        public void ShowDashboard()
        {
            CurrentView = _adminDashboardViewModel;
        }

        public void ShowMembers()
        {
            CurrentView = _adminMemberViewModel;
        }

        public void ShowTrainers()
        {
            CurrentView = _adminTrainerViewModel;
        }

        public void ShowClasses()
        {
            CurrentView = _adminClassesViewModel;
        }

        public void NotifyDataChanged() // The event that get's invoked
        {
            DataChanged?.Invoke();
        }
    }
}
