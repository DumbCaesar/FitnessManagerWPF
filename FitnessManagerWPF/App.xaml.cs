using System.Configuration;
using System.Data;
using System.Windows;
using FitnessManagerWPF.Services;

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
        }
    }
}
