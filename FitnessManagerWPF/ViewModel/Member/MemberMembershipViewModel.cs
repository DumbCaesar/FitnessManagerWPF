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
using System.Xml.Linq;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Member
{
    // =====================================
    //       MemberMembershipViewModel
    //        Author: Nicolaj + Oliver
    // =====================================
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
            UpdateIsActiveType();
            // Command for buying a membership
            BuyMembershipCommand = new RelayCommand(param => BuyMembership(param));
        }

        // =====================================
        //          BuyMembership()
        //          Author: Nicolaj
        // =====================================
        private void BuyMembership(object? param)
        {
            if (param is not Membership selectedMembership) return;
            var now = DateTime.Now;
            DateTime newExpiry;

            // Extend existing membership if same type and still active
            if (CurrentUser.ActiveMembership is not null && CurrentUser.ActiveMembership.Name == selectedMembership.Name && CurrentUser.HasActiveMembership)
            {
                newExpiry = CurrentUser.MembershipExpiresAt.Value.AddMonths(selectedMembership.DurationInMonths);
                MessageBox.Show($"Your membership was extended by {selectedMembership.DurationInMonths} months");
            }
            else
            {
                newExpiry = now.AddMonths(selectedMembership.DurationInMonths);
                if (CurrentUser.HasActiveMembership)
                {
                    var messageBox = MessageBox.Show($"Your current membership will end now and you will be changed to a {selectedMembership.Name} subscription", "Are you sure?", MessageBoxButton.OKCancel);
                    if (messageBox != MessageBoxResult.OK) return;
                }
                else
                {
                    MessageBox.Show($"You purchased a {selectedMembership.Name} subscription!");
                }
            }

            // Update user's membership info
            CurrentUser.MembershipExpiresAt = newExpiry;
            CurrentUser.ActiveMembership = selectedMembership;
            CurrentUser.ActiveMembershipId = selectedMembership.Id;
            CurrentUser.ActiveMembership = selectedMembership;

            // Record purchase
            var purchase = new Purchase
            {
                Id = ++_dataService.MaxSubscriptionId,
                Membership = selectedMembership,
                MembershipId = selectedMembership.Id,
                AmountPaid = selectedMembership.Price,
                PurchasedAt = now
            };

            CurrentUser.BillingHistory.Add(purchase);
            UserSubscriptions.Add(purchase);
            _dataService.SaveUsers();
            UpdateIsActiveType();
            _parentViewModel.NotifyDataChanged();
            UpdateMembershipEvent?.Invoke(); // Notify other views of change
        }

        // =====================================
        //        UpdateIsActiveType()
        //          Author: Nicolaj
        // =====================================
        private void UpdateIsActiveType()
        {
            foreach (Membership m in ListOfMemberships)
            {
                m.IsActiveType = CurrentUser.ActiveMembership?.Name == m.Name;
            }
        }
    }
}
