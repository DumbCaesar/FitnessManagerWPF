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
    public class SelectedClassViewModel : ObservableObject
    {
        private ObservableCollection<Classes> _listOfClasses;
        private ObservableCollection<User> _listOfUsers;
        private List<Classes> _classes;
        private Classes _selectedClass;
        private User _selectedUser;
        private AdminClassesViewModel _parentViewModel;
        private DataService _dataService;

        public Classes SelectedClass
        {
            get => _selectedClass;
            set
            {
               if(SetProperty(ref _selectedClass, value))
                {
                    ListOfUsers = _dataService.GetSelectedClass(SelectedClass);
                }
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public ObservableCollection<Classes> ListOfClasses
        {
            get => _listOfClasses;
            set => SetProperty(ref _listOfClasses, value);
        }

        public ObservableCollection<User> ListOfUsers
        {
            get => _listOfUsers;
            set => SetProperty(ref _listOfUsers, value);
        }

        public SelectedClassViewModel(DataService dataService, Classes classes)
        {
            _dataService = dataService;
            SelectedClass = classes;
            _classes = _dataService._activities;
            _listOfClasses = new ObservableCollection<Classes>(_classes);
            _listOfUsers = _dataService.GetSelectedClass(classes);

        }

        public SelectedClassViewModel()
        {
            
        }
    }
}
