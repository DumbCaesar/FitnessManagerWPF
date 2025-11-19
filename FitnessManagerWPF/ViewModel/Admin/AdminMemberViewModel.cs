using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class AdminMemberViewModel : ViewModelBase
    {
        private object _currentView;
        private AdminViewModel _parentViewModel;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public AdminMemberViewModel(AdminViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
        }
    }
}
