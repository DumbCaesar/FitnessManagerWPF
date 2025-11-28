using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class SelectedMemberViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private EditMemberViewModel _editMemberViewModel;
        private User _user;
        public event Action MemberChanged;

        public ICommand EditMemberCommand { get; set; }

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
            EditMemberCommand = new RelayCommand(_ => ShowEditMember());
            _dataService = dataService;
            SelectedMember = user;

        }

        private void LoadMemberDetails()
        {
            if (SelectedMember == null) return;
            _dataService.GetMemberDetails(SelectedMember.Id, MemberClasses, MembershipSubscriptions);
        }

        private void ShowEditMember()
        {
            _editMemberViewModel = new EditMemberViewModel(this, SelectedMember, _dataService);
            _editMemberViewModel.MemberChanged += UpdateMemberInfo;
            EditMemberView editMemberView = new EditMemberView { DataContext = _editMemberViewModel };
            editMemberView.ShowDialog();
            _editMemberViewModel.MemberChanged -= UpdateMemberInfo;
        }

        private void UpdateMemberInfo()
        {
            OnPropertyChanged(nameof(SelectedMember));
            MemberChanged?.Invoke();
        }


    }
}
