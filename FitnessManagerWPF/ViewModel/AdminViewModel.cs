using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.View.UserControl.Admin;
using FitnessManagerWPF.ViewModel.Admin;

namespace FitnessManagerWPF.ViewModel
{
    public class AdminViewModel : ViewModelBase
    {
        private User? _currentUser;
        private object _currentView;
        private AdminDashboardViewModel _adminDashboardViewModel;
        private AdminClassesViewModel _adminClassesViewModel;
        private AdminMemberViewModel _adminMemberViewModel;
        private AdminTrainerViewModel _adminTrainerViewModel;
        public ICommand DashboardCommand { get; set; }
        public ICommand MemberCommand { get; set; }
        public ICommand TrainerCommand { get; set; }
        public ICommand ClassesCommand { get; set; }
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty (ref _currentUser, value);
        }
        public AdminViewModel(User user)
        {
            _currentUser = user;
            _adminDashboardViewModel = new AdminDashboardViewModel(this);
            _adminClassesViewModel = new AdminClassesViewModel(this);
            _adminMemberViewModel = new AdminMemberViewModel(this);
            _adminTrainerViewModel = new AdminTrainerViewModel(this);
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            MemberCommand = new RelayCommand(_ => ShowMembers());
            TrainerCommand = new RelayCommand(_ => ShowTrainers());
            ClassesCommand = new RelayCommand(_ => ShowClasses());

            ShowDashboard();
        }

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

        public AdminViewModel()
        {

        }
    }
}
