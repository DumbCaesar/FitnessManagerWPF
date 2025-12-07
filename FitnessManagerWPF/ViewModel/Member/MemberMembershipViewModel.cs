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
    /// <summary>
    /// ViewModel for managing memberships for a member, including buying new memberships.
    /// </summary>
    public class MemberMembershipViewModel : ObservableObject
    {
        private DataService _dataService; // Access to memberships and users
        private MemberViewModel _parentViewModel;
        private Membership _selectedMembership; // Currently selected membership from UI
        private User _currentUser; // Logged in member
        private List<Membership> _listOfMemberships; // All available memberships
        private ObservableCollection<Purchase> _userSubscriptions; // User's purchase history

        public event Action UpdateMembershipEvent; // Notifies other views of membership updates

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
            // Initialize memberships and subscriptions
            _listOfMemberships = new List<Membership>(_dataService.Memberships);
            UserSubscriptions = new ObservableCollection<Purchase>(CurrentUser.BillingHistory);
            // Command for buying a membership
            BuyMembershipCommand = new RelayCommand(_ => BuyMembership());
        }

        private void BuyMembership()
        {
            var now = DateTime.Now;
            DateTime newExpiry;

            // Extend existing membership if same type and still active
            if (CurrentUser.ActiveMembership is not null && CurrentUser.ActiveMembership.Id == SelectedMembership.Id && CurrentUser.HasActiveMembership)
            {
                newExpiry = CurrentUser.MembershipExpiresAt.Value.AddMonths(SelectedMembership.DurationInMonths);
            }
            else
            {
                newExpiry = now.AddMonths(SelectedMembership.DurationInMonths);
            }

            // Update user's membership info
            CurrentUser.MembershipExpiresAt = newExpiry;
            CurrentUser.ActiveMembership = SelectedMembership;
            CurrentUser.ActiveMembershipId = SelectedMembership.Id;
            CurrentUser.ActiveMembership = SelectedMembership;

            // Record purchase
            var purchase = new Purchase
            {
                Id = ++_dataService.MaxSubscriptionId,
                Membership = SelectedMembership,
                MembershipId = SelectedMembership.Id,
                AmountPaid = SelectedMembership.Price,
                PurchasedAt = now
            };

            CurrentUser.BillingHistory.Add(purchase);
            UserSubscriptions.Add(purchase);
            _dataService.SaveUsers();
            UpdateMembershipEvent?.Invoke(); // Notify other views of change
        }
    }
}
