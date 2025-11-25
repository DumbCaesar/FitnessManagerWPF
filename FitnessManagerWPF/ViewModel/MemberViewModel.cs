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
    public class MemberViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private object _currentView;
        private MemberClassesViewModel _memberClassesViewModel;
        private MemberDashboardViewModel _memberDashboardViewModel;
        private MemberProfileViewModel _memberProfileViewModel;
        private User _currentUser;

        public ICommand DashboardCommand { get; set; }
        public ICommand ClassesCommand { get; set; }
        public ICommand ProfileCommand { get; set; }

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
            _memberDashboardViewModel = new MemberDashboardViewModel(this, _dataService);
            _memberClassesViewModel = new MemberClassesViewModel(this, _dataService);
            _memberProfileViewModel = new MemberProfileViewModel(this, _dataService, CurrentUser);
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            ClassesCommand = new RelayCommand(_ => ShowClasses());
            ProfileCommand = new RelayCommand(_ => ShowProfile());

            ShowDashboard();
        }

        public MemberViewModel() // Empty ctor used for DataContext in view
        {
            
        }

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
    }
}
