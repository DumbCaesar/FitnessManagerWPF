using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberDashboardViewModel
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
