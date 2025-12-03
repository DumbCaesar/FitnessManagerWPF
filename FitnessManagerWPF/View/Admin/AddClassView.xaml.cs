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
using FitnessManagerWPF.ViewModel.Admin;

namespace FitnessManagerWPF.View.Admin
{
    /// <summary>
    /// Interaction logic for AddClassView.xaml
    /// </summary>
    public partial class AddClassView : Window
    {
        public AddClassView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                if (DataContext is AddClassViewModel vm)
                    vm.CloseRequest += () =>
                    {
                        Close();
                    };
            };
        }
    }
}
