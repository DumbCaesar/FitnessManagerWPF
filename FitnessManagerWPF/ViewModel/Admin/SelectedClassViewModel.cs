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
using System.Windows;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Admin
{
    // =====================================
    //           SelectedClassViewModel
    //           Author: Oliver + Nicolaj
    // =====================================
    public class SelectedClassViewModel : ObservableObject
    {
        private ObservableCollection<GymClass> _gymClasses;
        private ObservableCollection<User> _listOfUsers;
        private GymClass _selectedClass;
        private User _selectedMember;
        private AdminClassesViewModel _parentViewModel;
        private DataService _dataService;
        public event Action<GymClass> ClassDeleted;

        public ICommand MemberDoubleClickCommand { get; set; }
        public ICommand DeleteClassCommand { get; set; }

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

        public ObservableCollection<User> ListOfUsers
        {
            get => _listOfUsers;
            set => SetProperty(ref _listOfUsers, value);
        }
        public ObservableCollection<GymClass> GymClasses
        {
            get => _gymClasses;
            set => SetProperty(ref _gymClasses, value);
        }
        public SelectedClassViewModel(DataService dataService, GymClass classes)
        {
            _dataService = dataService;
            SelectedClass = classes;
            // Get classes and users
            _gymClasses = new ObservableCollection<GymClass>(_dataService.GymClasses);
            _listOfUsers = _dataService.GetSelectedClass(classes);
            MemberDoubleClickCommand = new RelayCommand(_ => ShowSelectedMember());
            DeleteClassCommand = new RelayCommand(_ => DeleteClass());
        }

        // =====================================
        //           ShowSelectedMember()
        //           Author: Oliver
        // =====================================
        private void ShowSelectedMember()
        {
            if (SelectedMember == null) return;
            // Show the selected class from the list
            SelectedMemberViewModel selectedMemberViewModel = new SelectedMemberViewModel(SelectedMember, _dataService);
            SelectedMemberView selectedMemberView = new SelectedMemberView { DataContext = selectedMemberViewModel };
            selectedMemberView.ShowDialog();
        }

        // =====================================
        //           DeleteClass()
        //           Author: Nicolaj
        // =====================================
        private void DeleteClass()
        {
            GymClass classToDelete = SelectedClass;

            var messageBox = MessageBox.Show($"Are you sure you want to delete {SelectedClass.Name}?", "Are you sure?", MessageBoxButton.OKCancel);
            if (messageBox != MessageBoxResult.OK) return;
            GymClasses.Remove(classToDelete);

            _dataService.GymClasses.Remove(classToDelete);
            _dataService.SaveGymClasses();
            ClassDeleted?.Invoke(classToDelete);
        }
    }
}
