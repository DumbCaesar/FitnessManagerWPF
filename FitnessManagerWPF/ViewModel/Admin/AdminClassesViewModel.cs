using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View;
using FitnessManagerWPF.View.Admin;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminClassesViewModel : ObservableObject
    {
        private object _currentView;
        private string _activityStats;
        private DataService _dataService;
        private AdminViewModel _parentViewModel;
        private Classes? _selectedActivity;
        private List<Classes> _activityList;
        private ObservableCollection<Classes> _activities;
        public ICommand AddClassCommand { get; set; }

        public Classes? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                SetProperty(ref _selectedActivity, value);
                Debug.WriteLine($"Selected Activity is: {_selectedActivity.Name}");
            }
        }

        public ObservableCollection<Classes> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminClassesViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _activityList = new List<Classes>();
            _dataService = dataService;
            _activityList = _dataService._activities.ToList();
            _activities = new ObservableCollection<Classes>(_activityList);
            AddClassCommand = new RelayCommand(_ => AddClass());
        }

        private void AddClass()
        {
            Debug.WriteLine("Clicked add class");
            AddClassViewModel addClassViewModel = new AddClassViewModel(_dataService);
            addClassViewModel.ClassCreated += OnClassCreated;

            var addClassView = new AddClassView { DataContext = addClassViewModel };
            addClassView.Show();
        }

        private void OnClassCreated(Classes newClass) => Activities.Add(newClass);
    }
}
