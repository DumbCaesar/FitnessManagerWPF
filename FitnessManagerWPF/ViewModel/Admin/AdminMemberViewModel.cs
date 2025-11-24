using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminMemberViewModel : ViewModelBase
    {
        private object _currentView;
        private User _selectedMember;
        private AdminViewModel _parentViewModel;
        private SelectedMemberViewModel _selectedMemberViewModel;
        private ObservableCollection<User> _listOfMembers;
        private List<User> _userList;
        private DataService _dataService;

        public ICommand MemberDoubleClickCommand { get; set; }

        public SelectedMemberViewModel SelectedMemberViewModel
        {
            get => _selectedMemberViewModel;
            set => SetProperty(ref _selectedMemberViewModel, value);
        }

        public ObservableCollection<User> MemberList
        {
            get => _listOfMembers;
            set => SetProperty(ref _listOfMembers, value);
        }

        public User SelectedMember
        {
            get => _selectedMember;
            set 
            {
                SetProperty(ref _selectedMember, value);
                Debug.WriteLine($"Selected Member is: {_selectedMember.Name}");

            }
        } 

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminMemberViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            MemberDoubleClickCommand = new RelayCommand(_ => OnMemberDoubleClick());
            _parentViewModel = parentViewModel;
            _userList = new List<User>();
            _dataService = dataService;
            _userList = _dataService.Users;
            _listOfMembers = new ObservableCollection<User>(_userList.Where(u => u.UserRole == UserRole.Member));
        }

        private void OnMemberDoubleClick()
        {
            if (SelectedMember == null) return;
            SelectedMemberViewModel = new SelectedMemberViewModel(SelectedMember, _dataService);
            var SelectedMemberView = new SelectedMemberView { DataContext = SelectedMemberViewModel };
            SelectedMemberView.Show();
        }

        
    }
}
