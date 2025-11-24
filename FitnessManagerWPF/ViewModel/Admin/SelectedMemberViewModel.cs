using FitnessManagerWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessManagerWPF.ViewModel.Admin
{
    public class SelectedMemberViewModel : ViewModelBase
    {
        public User SelectedMember { get; set; }

        public SelectedMemberViewModel(User user)
        {
            SelectedMember = user;
        }
    }
}
