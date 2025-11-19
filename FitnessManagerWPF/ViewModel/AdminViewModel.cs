using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.View.UserControl.Admin;

namespace FitnessManagerWPF.ViewModel
{
    public class AdminViewModel : ViewModelBase
    {
        private User? _currentUser;
        private object _currentView;
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
            DashboardCommand = new RelayCommand(_ => ShowDashboard());
            MemberCommand = new RelayCommand(_ => ShowMembers());
            TrainerCommand = new RelayCommand(_ => ShowTrainers());
            ClassesCommand = new RelayCommand(_ => ShowClasses());

            ShowDashboard();
        }

        public void ShowDashboard()
        {
            CurrentView = new AdminDashboardView();
        }

        public void ShowMembers()
        {
            CurrentView = new AdminMemberView();
        }

        public void ShowTrainers()
        {
            CurrentView = new AdminTrainerView();
        }

        public void ShowClasses()
        {
            CurrentView = new AdminClassesView();
        }

        public AdminViewModel()
        {

        }
    }
}
