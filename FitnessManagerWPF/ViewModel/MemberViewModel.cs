using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.ViewModel.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel
{
    /// <summary>
    /// Manages navigation and state for the Member views.
    /// </summary>
    public class MemberViewModel : ObservableObject
    {
        private readonly DataService _dataService; // Access to data layer
        private object _currentView; // Currently displayed member view

        // Sub-viewmodels for each member page
        private MemberClassesViewModel _memberClassesViewModel;
        private MemberDashboardViewModel _memberDashboardViewModel;
        private MemberProfileViewModel _memberProfileViewModel;
        private MemberMembershipViewModel _memberMembershipViewModel;
        private User _currentUser; // Logged-in member

        public event Action DataChanged; // Event for updating UI when underlying data updates.

        // Commands for navigation
        public ICommand DashboardCommand { get; set; }
        public ICommand ClassesCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand MembershipCommand { get; set; }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }
        public MemberViewModel(User currentUser, DataService dataService)
        {
            _currentUser = currentUser;
            _dataService = dataService;

            // Initialize sub-viewmodels
            _memberDashboardViewModel = new MemberDashboardViewModel(this, _dataService);
            _memberClassesViewModel = new MemberClassesViewModel(this, _dataService);
            _memberProfileViewModel = new MemberProfileViewModel(this, _dataService, CurrentUser);
            _memberMembershipViewModel = new MemberMembershipViewModel(this, _dataService, CurrentUser);

            // Setup navigation commands
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            ClassesCommand = new RelayCommand(_ => ShowClasses());
            ProfileCommand = new RelayCommand(_ => ShowProfile());
            MembershipCommand = new RelayCommand(_ => ShowMembership());

            // Update profile role when membership changes
            _memberMembershipViewModel.UpdateMembershipEvent += _memberProfileViewModel.UpdateMemberRole;

            ShowDashboard(); // Default page
        }

        public MemberViewModel() 
        {
            
        }

        // The command methods for switching to a different view.
        private void ShowDashboard()
        {
            CurrentView = _memberDashboardViewModel;
        }

        private void ShowClasses()
        {
            CurrentView = _memberClassesViewModel;
        }

        private void ShowProfile()
        {
            CurrentView = _memberProfileViewModel;
        }

        private void ShowMembership()
        {
            CurrentView = _memberMembershipViewModel;
        }

        public void NotifyDataChanged() // The event that get's invoked
        {
            DataChanged?.Invoke();
        }
    }
}
