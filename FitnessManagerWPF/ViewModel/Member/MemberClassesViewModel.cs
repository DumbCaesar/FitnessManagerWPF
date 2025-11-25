using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessManagerWPF.Model;
using FitnessManagerWPF.Services;

namespace FitnessManagerWPF.ViewModel.Member
{
    public class MemberClassesViewModel
    {
        private DataService _dataService;
        private MemberViewModel _parentViewModel;
        public ObservableCollection<Classes> Classes => new ObservableCollection<Classes>(_dataService._activities);
        public MemberClassesViewModel(MemberViewModel parentViewModel, DataService dataService)
        {
            _parentViewModel = parentViewModel;
            _dataService = dataService;
        }
    }
}
