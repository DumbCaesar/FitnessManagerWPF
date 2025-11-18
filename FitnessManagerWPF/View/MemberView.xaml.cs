using FitnessManagerWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FitnessManagerWPF.View
{
    /// <summary>
    /// Interaction logic for MemberView.xaml
    /// </summary>
    public partial class MemberView : Window
    {
        private MemberViewModel viewModel;
        public MemberView()
        {
            InitializeComponent();
            viewModel = new MemberViewModel();
            DataContext = viewModel;
        }
    }
}
