using System.Configuration;
using System.Data;
using System.Windows;
using FitnessManagerWPF.Services;
using FitnessManagerWPF.View;

namespace FitnessManagerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DataService DataService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DataService = new DataService();

            var loginView = new LoginView();
            loginView.Show();
        }
    }
}
