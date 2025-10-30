using System.Windows;
using Microsoft.Win32;

namespace KioskBrowser
{
    public partial class SettingsWindow : Window
    {
        private readonly KioskConfiguration _config;

        public SettingsWindow(KioskConfiguration config)
        {
            InitializeComponent();
            _config = config;
            LoadSettings();
        }

        private void LoadSettings()
        {
            HomePageTextBox.Text = _config.HomePage;
            StartFullscreenCheckBox.IsChecked = _config.StartFullscreen;
            ShowKeyboardCheckBox.IsChecked = _config.ShowVirtualKeyboard;
            EnableContextMenuCheckBox.IsChecked = _config.EnableContextMenu;
            EnableZoomCheckBox.IsChecked = _config.EnableZoom;
            EnablePrintingCheckBox.IsChecked = _config.EnablePrinting;
            IdleTimeoutTextBox.Text = _config.IdleTimeoutMinutes.ToString();
            BlockedDomainsTextBox.Text = _config.BlockedDomains;
            AllowDownloadsCheckBox.IsChecked = _config.AllowDownloads;
            DownloadPathTextBox.Text = _config.DownloadPath;

            UpdateDownloadControls();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.HomePage = HomePageTextBox.Text;
                _config.StartFullscreen = StartFullscreenCheckBox.IsChecked ?? true;
                _config.ShowVirtualKeyboard = ShowKeyboardCheckBox.IsChecked ?? true;
                _config.EnableContextMenu = EnableContextMenuCheckBox.IsChecked ?? false;
                _config.EnableZoom = EnableZoomCheckBox.IsChecked ?? true;
                _config.EnablePrinting = EnablePrintingCheckBox.IsChecked ?? true;
                _config.BlockedDomains = BlockedDomainsTextBox.Text;
                _config.AllowDownloads = AllowDownloadsCheckBox.IsChecked ?? false;
                _config.DownloadPath = DownloadPathTextBox.Text;

                if (int.TryParse(IdleTimeoutTextBox.Text, out int timeout))
                {
                    _config.IdleTimeoutMinutes = timeout;
                }

                _config.Save();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to reset all settings to defaults?",
                "Reset Settings",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var defaultConfig = new KioskConfiguration();
                _config.HomePage = defaultConfig.HomePage;
                _config.StartFullscreen = defaultConfig.StartFullscreen;
                _config.ShowVirtualKeyboard = defaultConfig.ShowVirtualKeyboard;
                _config.EnableContextMenu = defaultConfig.EnableContextMenu;
                _config.EnableZoom = defaultConfig.EnableZoom;
                _config.EnablePrinting = defaultConfig.EnablePrinting;
                _config.IdleTimeoutMinutes = defaultConfig.IdleTimeoutMinutes;
                _config.BlockedDomains = defaultConfig.BlockedDomains;
                _config.AllowDownloads = defaultConfig.AllowDownloads;
                _config.DownloadPath = defaultConfig.DownloadPath;

                LoadSettings();
            }
        }

        private void AllowDownloadsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateDownloadControls();
        }

        private void UpdateDownloadControls()
        {
            bool isEnabled = AllowDownloadsCheckBox.IsChecked ?? false;
            DownloadPathTextBox.IsEnabled = isEnabled;
            BrowseButton.IsEnabled = isEnabled;
        }

        private void BrowseDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Download Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                DownloadPathTextBox.Text = dialog.FolderName;
            }
        }
    }
}
