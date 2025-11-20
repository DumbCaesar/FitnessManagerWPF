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
    public class AdminMemberViewModel : ViewModelBase
    {
        private User _selectedMember;
        private object _currentView;
        private AdminViewModel _parentViewModel;
        private ObservableCollection<User> _listOfMembers;
        private List<User> _userList;
        private DataService _dataService;

        public ObservableCollection<User> MemberList
        {
            get => _listOfMembers;
            set => SetProperty(ref _listOfMembers, value);
        }

        public User SelectedMember
        {
            get => _selectedMember;
            set => SetProperty(ref _selectedMember, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminMemberViewModel(AdminViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _userList = new List<User>();
            _dataService = new DataService();
            _dataService.LoadData();
            _userList = _dataService.Users;
            _listOfMembers = new ObservableCollection<User>(_userList.Where(u => u.UserRole == UserRole.Member));
        }

        
    }
}
