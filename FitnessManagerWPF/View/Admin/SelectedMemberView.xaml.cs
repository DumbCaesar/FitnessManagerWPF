using FitnessManagerWPF.ViewModel.Admin;
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

namespace FitnessManagerWPF.View.Admin
{
    /// <summary>
    /// Interaction logic for SelectedMemberView.xaml
    /// </summary>
    public partial class SelectedMemberView : Window
    {
        private SelectedMemberViewModel viewModel;
        public SelectedMemberView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SelectedMemberViewModel vm)
            {
                viewModel = vm;
                viewModel.MemberDeleted += OnDeleteMember;
            }
            else
            {
                throw new InvalidOperationException(
                    "SelectedMemberView must be created with a SelectedMemberViewModel as DataContext.");
            }
        }

        private void OnDeleteMember()
        {
            viewModel.MemberDeleted -= OnDeleteMember;
            Close();
        }
    }
}
