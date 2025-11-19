using FitnessManagerWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel
{
    public class TrainerViewModel : ViewModelBase
    {
        private User? _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public TrainerViewModel(User currentUser)
        {
            _currentUser = currentUser;
        }

        public TrainerViewModel()
        {
            
        }
    }
}
