using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices;
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
        private ObservableCollection<Purchase> _userSubscriptions;

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

        public ObservableCollection<Purchase> UserSubscriptions
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
            UserSubscriptions = new ObservableCollection<Purchase>(CurrentUser.BillingHistory);
            BuyMembershipCommand = new RelayCommand(_ => BuyMembership());
        }

        private void BuyMembership()
        {
            var now = DateTime.Now;

            DateTime newExpiry;
            if (CurrentUser.ActiveMembership is not null && CurrentUser.ActiveMembership.Id == SelectedMembership.Id && CurrentUser.HasActiveMembership)
            {
                newExpiry = CurrentUser.MembershipExpiresAt.AddMonths(SelectedMembership.DurationInMonths);
            }
            else
            {
                newExpiry = now.AddMonths(SelectedMembership.DurationInMonths);
            }

            CurrentUser.MembershipExpiresAt = newExpiry;
            CurrentUser.ActiveMembership = SelectedMembership;
            CurrentUser.ActiveMembershipId = SelectedMembership.Id;
            CurrentUser.ActiveMembership = SelectedMembership;

            var purchase = new Purchase
            {
                Id = ++_dataService.MaxSubscriptionId,
                Membership = SelectedMembership,
                MembershipId = SelectedMembership.Id,
                AmountPaid = SelectedMembership.Price,
                PurchasedAt = now
            };

            CurrentUser.BillingHistory.Add(purchase);
            _dataService.SaveMembers();
            UpdateMembershipEvent?.Invoke();
        }
    }
}
