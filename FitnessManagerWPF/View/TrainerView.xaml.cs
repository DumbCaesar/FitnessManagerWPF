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
    /// Interaction logic for TrainerView.xaml
    /// </summary>
    public partial class TrainerView : Window
    {
        private TrainerViewModel viewModel;
        public TrainerView()
        {
            InitializeComponent();
            viewModel = new TrainerViewModel();
            DataContext = viewModel;
        }
    }
}
