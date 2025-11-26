using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberDashboardViewModel : ObservableObject
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;

        public MemberDashboardViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
        }

        
    }
}
