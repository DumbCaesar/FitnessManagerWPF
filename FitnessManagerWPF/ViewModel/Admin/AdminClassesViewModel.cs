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
        private DataService _dataService;
        private AdminViewModel _parentViewModel;
        private GymClass? _selectedClass;
        private ObservableCollection<GymClass> _gymClasses;
        // Raised when a new class is created from the AddClass popup
        public event Action ClassCreated;
        public ICommand AddClassCommand { get; set; }
        public ICommand ShowSelectedClassCommand { get; set; }

        public GymClass? SelectedClass
        {
            get => _selectedClass;
            set
            {
                SetProperty(ref _selectedClass, value);
                // for debugging / UI state tracking
                if (SelectedClass != null)
                {
                    Debug.WriteLine($"Selected class is: {_selectedClass?.Name}");
                }
            }
        }

        public ObservableCollection<GymClass> GymClasses
        {
            get => _gymClasses;
            set => SetProperty(ref _gymClasses, value);
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
            // Load classes initially from DataService storage
            _gymClasses = new ObservableCollection<GymClass>(_dataService.GymClasses);
            AddClassCommand = new RelayCommand(_ => AddClass());
            ShowSelectedClassCommand = new RelayCommand(_ => ShowSelectedClass());
        }

        private void AddClass()
        {
            Debug.WriteLine("Clicked add class");
            // Create the popup view model and subscribe to event before showing window
            AddClassViewModel addClassViewModel = new AddClassViewModel(_dataService);
            addClassViewModel.ClassCreated += OnClassCreated;

            var view = new AddClassView { DataContext = addClassViewModel };
            view.ShowDialog();
            // Cleanup to avoid memory leaks
            addClassViewModel.ClassCreated -= OnClassCreated;
        }

        private void ShowSelectedClass()
        {
            if (SelectedClass == null) return;
            // Opens details window for the selected class
            SelectedClassViewModel selectedClassViewModel = new SelectedClassViewModel(_dataService, SelectedClass);
            SelectedClassView selectedClassView = new SelectedClassView { DataContext = selectedClassViewModel };
            selectedClassView.ShowDialog();
        }

        private void OnClassCreated(GymClass newClass)
        {
            // Add newly created class to the list and notify parent viewmodel to raise dashboard update event.
            GymClasses.Add(newClass);
            _parentViewModel.NotifyDataChanged();
            ClassCreated?.Invoke();
        }
    }
}
