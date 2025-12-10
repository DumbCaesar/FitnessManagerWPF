using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    // =====================================
    //       SelectedMemberViewModel()
    //       Author: Oliver + Nicolaj
    // =====================================
    public class SelectedMemberViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private User _user;
        // Events for MemberChanged and deleted.
        public event Action MemberChanged;
        public event Action MemberDeleted;

        public ICommand EditMemberCommand { get; set; }
        public ICommand DeleteMemberCommand { get; set; }

        public ObservableCollection<GymClass> MemberClasses { get; set; }
        public ObservableCollection<Purchase> MembershipSubscriptions { get; set; }

        public User SelectedMember
        {
            get => _user;
            set
            {
                SetProperty(ref _user, value);
            }
        }

        public SelectedMemberViewModel(User user, DataService dataService)
        {
            _dataService = dataService;
            SelectedMember = user;
            // Get the classes the Selected member is signed up for.
            var memberClasses = _dataService.GymClasses
                .Where(c => c.RegisteredMemberIds != null && c.RegisteredMemberIds.Contains(user.Id));
            // add classes and billing history
            MemberClasses = new ObservableCollection<GymClass>(memberClasses);
            MembershipSubscriptions = new ObservableCollection<Purchase>(user.BillingHistory);

            EditMemberCommand = new RelayCommand(_ => ShowEditMember());
            DeleteMemberCommand = new RelayCommand(_ => OnDeleteMember());
        }

        // =====================================
        //           ShowEditMember()
        //           Author: Oliver
        // =====================================
        private void ShowEditMember()
        {
            if(SelectedMember == null) return;
            // ShowDialog for the selected member, to edit info
            var _editMemberViewModel = new EditMemberViewModel(this, SelectedMember, _dataService);
            _editMemberViewModel.MemberChanged += OnUpdateMemberInfo;
            EditMemberView editMemberView = new EditMemberView { DataContext = _editMemberViewModel };
            editMemberView.ShowDialog();
            _editMemberViewModel.MemberChanged -= OnUpdateMemberInfo;
        }

        // =====================================
        //           OnDeleteMember()
        //           Author: Oliver
        // =====================================
        private void OnDeleteMember()
        {
            if (SelectedMember == null) return;
            // remove a user, double confirmation required
            var messageBox = MessageBox.Show($"Are you sure you want to delete {SelectedMember.Name}?", "Permanant Action", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if(messageBox == MessageBoxResult.OK)
            {
                var messageBoxConfirm = MessageBox.Show($"Are you completely sure? {SelectedMember.Name} will be removed permanently.", "Are you sure?", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if(messageBoxConfirm == MessageBoxResult.OK)
                {
                    _dataService.DeleteMember(SelectedMember);
                    MemberDeleted?.Invoke();
                }
                return;
            }
            return;
        }

        // =====================================
        //          OnUpdateMemberInfo()
        //           Author: Oliver
        // =====================================
        // Invoke event if member info is updated
        private void OnUpdateMemberInfo()
        {
            OnPropertyChanged(nameof(SelectedMember));
            MemberChanged?.Invoke();
        }


    }
}
