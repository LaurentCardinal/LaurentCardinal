using System.Windows;

namespace KioskBrowser
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle any unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MessageBox.Show(
                    $"An unexpected error occurred: {args.ExceptionObject}",
                    "Kiosk Browser Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };
        }
    }
}
