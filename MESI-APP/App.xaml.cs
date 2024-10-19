using MESI_APP.Http;
using MESI_APP.ViewModels;
using MESI_APP.Views;
using Microsoft.Extensions.DependencyInjection;

using System.Windows;

namespace MESI_APP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
            ServiceCollection services = new ServiceCollection();
            services.ConfigureServices();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }

    public static class ServiceCollectionExtensions {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ServerService>();
            services.AddSingleton<ClientService>();
        }
    }

}
