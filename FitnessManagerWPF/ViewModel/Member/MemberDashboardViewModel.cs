using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Member
{
    // =====================================
    //       MemberDashboardViewModel
    //       Author: Nicolaj + Oliver
    // =====================================
    public class MemberDashboardViewModel : ObservableObject
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;
        private User _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string CurrentDate
        {
            get => DateTime.Now.ToString();
        }

        public string WelcomeMessage
        {
            get => $"Welcome {_currentUser.Name}";
        }

        public string MembershipStatusDisplay
        {
            get => CurrentUser.HasActiveMembership? $"You have {CurrentUser.MembershipExpiresDisplay} of your {CurrentUser.MembershipStatusDisplay} membership" : "";
        }

        public MemberDashboardViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
            CurrentUser = _parentViewModel.CurrentUser;
        }
        public GymClass NextClass => _dataService.GymClasses?.Where(c => c.RegisteredMemberIds.Contains(_currentUser.Id)).OrderBy(c => c.NextOccurrence).FirstOrDefault();
        public GymClass NextGymClass => _dataService.GymClasses?.OrderBy(c => c.NextOccurrence).FirstOrDefault();

    }
}
