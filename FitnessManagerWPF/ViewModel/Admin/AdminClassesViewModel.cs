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
        private GymClass? _selectedClass;
        private ObservableCollection<GymClass> _gymClasses;

        public event Action ClassCreated;
        public ICommand AddClassCommand { get; set; }
        public ICommand ShowSelectedClassCommand { get; set; }

        public GymClass? SelectedClass
        {
            get => _selectedClass;
            set
            {
                SetProperty(ref _selectedClass, value);
                if (SelectedClass != null)
                {
                    Debug.WriteLine($"Selected class is: {_selectedClass.Name}");
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
            _gymClasses = new ObservableCollection<GymClass>(_dataService.GymClasses);
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
            if (SelectedClass == null) return;
            SelectedClassViewModel selectedClassViewModel = new SelectedClassViewModel(_dataService, SelectedClass);
            SelectedClassView selectedClassView = new SelectedClassView { DataContext = selectedClassViewModel };
            selectedClassView.ShowDialog();
        }

        private void OnClassCreated(GymClass newClass)
        {
            GymClasses.Add(newClass);
            _parentViewModel.NotifyDataChanged();
            ClassCreated?.Invoke();
        }
    }
}
