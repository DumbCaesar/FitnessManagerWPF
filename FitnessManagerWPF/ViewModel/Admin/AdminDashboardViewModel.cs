using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private readonly DataService _dataService;
        private object _currentView;
        private AdminViewModel _parentViewModel;
        private string _memberCount;
        private string _memberMonthlyRevenue;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminDashboardViewModel(AdminViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
        }
    }
}
