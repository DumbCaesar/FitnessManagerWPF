using FitnessManagerWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberProfileViewModel
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;

        public MemberProfileViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
        }
    }
}
