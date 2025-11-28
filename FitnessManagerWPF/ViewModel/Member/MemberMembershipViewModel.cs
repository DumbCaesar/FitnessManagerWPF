using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberMembershipViewModel : ObservableObject
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;
        private Membership _selectedMembership;
        private User _currentUser;
        private List<Membership> _listOfMemberships;
        private ObservableCollection<MembershipSubscription> _userSubscriptions;

        public event Action UpdateMembershipEvent;

        public ICommand BuyMembershipCommand { get; }

        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public Membership SelectedMembership
        {
            get => _selectedMembership;
            set 
            {
                SetProperty(ref _selectedMembership, value);
                Debug.WriteLine($"Selected Membership: {SelectedMembership.Name}");
            }
        }

        public ObservableCollection<MembershipSubscription> UserSubscriptions
        {
            get => _userSubscriptions;
            set => SetProperty(ref _userSubscriptions, value);
        }

        public List<Membership> ListOfMemberships
        {
            get => _listOfMemberships;
            set => _listOfMemberships = value;
        }

        public MemberMembershipViewModel(MemberViewModel parentViewModel, DataService dataService, User user)
        {
            _dataService = dataService;
            _parentViewModel = parentViewModel;
            _currentUser = user;
            _listOfMemberships = new List<Membership>(_dataService.Memberships);
            UserSubscriptions = new ObservableCollection<MembershipSubscription>(CurrentUser.BillingHistory);
            BuyMembershipCommand = new RelayCommand(_ => BuyMembership());
        }

        private void BuyMembership()
        {
            if(SelectedMembership == null)
            {
                MessageBox.Show("A membership needs to be selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentUser.IsActiveMember && SelectedMembership.Id != CurrentUser.CurrentMembership().MembershipId)
            {
                CurrentUser.CurrentMembership().EndDate = DateTime.Now;
            }
            var subscription = new MembershipSubscription(SelectedMembership, ++_dataService.MaxSubscriptionId);
            CurrentUser.BillingHistory.Add(subscription);
            UserSubscriptions = new ObservableCollection<MembershipSubscription>(CurrentUser.BillingHistory);
            Debug.WriteLine($"Sucess! {SelectedMembership.Name} bought.");
            Debug.WriteLine($"{CurrentUser.Name} is now {CurrentUser.MembershipTypeDisplay}");
            _dataService.SaveMembers();
            UpdateMembershipEvent?.Invoke();
        }
    }
}
