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
    public class SelectedMemberViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private User _user;

        public ObservableCollection<Classes> MemberClasses { get; set; }
        public ObservableCollection<MembershipSubscription> MembershipSubscriptions { get; set; }

        public User SelectedMember
        {
            get => _user;
            set
            {
                SetProperty(ref _user, value);
                LoadMemberDetails();
            }
        }

        public SelectedMemberViewModel(User user, DataService dataService)
        {
            MemberClasses = new ObservableCollection<Classes>();
            MembershipSubscriptions = new ObservableCollection<MembershipSubscription>();
            _dataService = dataService;
            SelectedMember = user;
        }

        private void LoadMemberDetails()
        {
            if (SelectedMember == null) return;
            _dataService.GetMemberDetails(SelectedMember.Id, MemberClasses, MembershipSubscriptions);
        }


    }
}
