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
        private ObservableCollection<Classes> _activities;

        public event Action ClassCreated;
        public ICommand AddClassCommand { get; set; }
        public ICommand ShowSelectedClassCommand { get; set; }

        public Classes? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                SetProperty(ref _selectedActivity, value);
                if (SelectedActivity != null)
                {
                    Debug.WriteLine($"Selected Activity is: {_selectedActivity.Name}");
                }
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
            _dataService = dataService;
            _activities = new ObservableCollection<Classes>(_dataService.Activities);
            AddClassCommand = new RelayCommand(_ => AddClass());
            ShowSelectedClassCommand = new RelayCommand(_ => ShowSelectedClass());
        }

        private void AddClass()
        {
            Debug.WriteLine("Clicked add class");
            AddClassViewModel addClassViewModel = new AddClassViewModel(_dataService);
            addClassViewModel.ClassCreated += OnClassCreated;

            var view = new AddClassView { DataContext = addClassViewModel };
            view.ShowDialog();

            addClassViewModel.ClassCreated -= OnClassCreated;
        }

        private void ShowSelectedClass()
        {
            if (SelectedActivity == null) return;
            SelectedClassViewModel selectedClassViewModel = new SelectedClassViewModel(_dataService, SelectedActivity);
            SelectedClassView selectedClassView = new SelectedClassView { DataContext = selectedClassViewModel };
            selectedClassView.ShowDialog();
        }

        private void OnClassCreated(Classes newClass)
        {
            Activities.Add(newClass);
            _parentViewModel.NotifyDataChanged();
            ClassCreated?.Invoke();
        }
    }
}
