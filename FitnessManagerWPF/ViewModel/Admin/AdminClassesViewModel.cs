using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminClassesViewModel : ViewModelBase
    {
        private object _currentView;
        private string _activityStats;
        private DataService _dataService;
        private AdminViewModel _parentViewModel;
        private Classes? _selectedActivity;
        private List<Classes> _activityList;
        private ObservableCollection<Classes> _activities;

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

        public AdminClassesViewModel(AdminViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            _activityList = new List<Classes>();
            _dataService = new DataService();
            _dataService.LoadData();
            _activityList = _dataService._activities.ToList();
            _activities = new ObservableCollection<Classes>(_activityList);

        }
    }
}
