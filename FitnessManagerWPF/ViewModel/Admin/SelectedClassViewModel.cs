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
    public class SelectedClassViewModel : ObservableObject
    {
        private ObservableCollection<GymClass> _listOfClasses;
        private ObservableCollection<User> _listOfUsers;
        private List<GymClass> _gymClasses;
        private GymClass _selectedClass;
        private User _selectedMember;
        private AdminClassesViewModel _parentViewModel;
        private DataService _dataService;

        public ICommand MemberDoubleClickCommand { get; set; }

        public GymClass SelectedClass
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

        public User SelectedMember
        {
            get => _selectedMember;
            set
            {
                SetProperty(ref _selectedMember, value);
                Debug.WriteLine($"Selcted Member is: {SelectedMember.Name}");
            }
        }

        public ObservableCollection<GymClass> ListOfClasses
        {
            get => _listOfClasses;
            set => SetProperty(ref _listOfClasses, value);
        }

        public ObservableCollection<User> ListOfUsers
        {
            get => _listOfUsers;
            set => SetProperty(ref _listOfUsers, value);
        }

        public SelectedClassViewModel(DataService dataService, GymClass classes)
        {
            _dataService = dataService;
            SelectedClass = classes;
            _gymClasses = _dataService.GymClasses;
            _listOfClasses = new ObservableCollection<GymClass>(_gymClasses);
            _listOfUsers = _dataService.GetSelectedClass(classes);
            MemberDoubleClickCommand = new RelayCommand(_ => ShowSelectedMember());

        }

        private void ShowSelectedMember()
        {
            if (SelectedMember == null) return;
            SelectedMemberViewModel selectedMemberViewModel = new SelectedMemberViewModel(SelectedMember, _dataService);
            SelectedMemberView selectedMemberView = new SelectedMemberView { DataContext = selectedMemberViewModel };
            selectedMemberView.ShowDialog();
        }
    }
}
