using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel.Admin
{
    // =====================================
    //           AdminTrainerViewModel
    //           Author: Oliver + Nicolaj
    // =====================================
    public class AdminTrainerViewModel : ObservableObject
    {
        private User _selectedTrainer;
        private List<User> _trainerList;
        private ObservableCollection<User> _listOfTrainers;
        private DataService _dataService;
        private object _currentView;
        private AdminViewModel _parentViewModel;

        public ObservableCollection<User> TrainerList
        {
            get => _listOfTrainers;
            set => SetProperty(ref _listOfTrainers, value);
        }

        public User SelectedTrainer
        {
            get => _selectedTrainer;
            set => SetProperty(ref _selectedTrainer, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminTrainerViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _trainerList = new List<User>();
            _dataService = dataService;
            // add them to list and observableCollection
            _trainerList = GetTrainers();
            _listOfTrainers = new ObservableCollection<User>(_trainerList);
        }

        // =====================================
        //           GetTrainers()
        //           Author: Nicolaj
        // =====================================
        private List<User> GetTrainers()
        {
            // Return all trainers
            return _dataService.Users.Where(u => u.UserRole == UserRole.Trainer).ToList();
        }
    }
}
