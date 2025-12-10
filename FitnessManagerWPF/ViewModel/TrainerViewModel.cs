using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel
{
    // Not currently used for anything, as Trainer section of the app has been shelved.
    public class TrainerViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private User _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public TrainerViewModel(User currentUser, DataService dataService)
        {
            _currentUser = currentUser;
            _dataService = dataService;
        }

        public TrainerViewModel()
        {
            
        }
    }
}
