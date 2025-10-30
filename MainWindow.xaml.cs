using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Web.WebView2.Core;
using System.Configuration;
using System.Runtime.InteropServices;

namespace KioskBrowser
{
    public partial class MainWindow : Window
    {
        private readonly KioskConfiguration _config;
        private bool _isFullscreen = true;
        private bool _adminMode = false;
        private int _adminKeyPressCount = 0;
        private System.Windows.Threading.DispatcherTimer? _adminKeyTimer;
        private readonly HashSet<string> _blockedDomains = new();

        public MainWindow()
        {
            InitializeComponent();
            _config = KioskConfiguration.Load();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                // Initialize WebView2
                await WebBrowser.EnsureCoreWebView2Async();

                // Configure WebView2 settings
                WebBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = _config.EnableContextMenu;
                WebBrowser.CoreWebView2.Settings.AreDevToolsEnabled = false;
                WebBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                WebBrowser.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                WebBrowser.CoreWebView2.Settings.IsWebMessageEnabled = true;
                WebBrowser.CoreWebView2.Settings.IsZoomControlEnabled = _config.EnableZoom;

                // Set up navigation events
                WebBrowser.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                WebBrowser.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                WebBrowser.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;

                // Load blocked domains
                LoadBlockedDomains();

                // Navigate to home page
                if (!string.IsNullOrWhiteSpace(_config.HomePage))
                {
                    UrlTextBox.Text = _config.HomePage;
                    WebBrowser.Source = new Uri(_config.HomePage);
                }

                // Set up fullscreen mode
                if (_config.StartFullscreen)
                {
                    EnterFullscreen();
                }

                // Set up virtual keyboard visibility
                if (_config.ShowVirtualKeyboard)
                {
                    KeyboardButton.Visibility = Visibility.Visible;
                }

                // Set up idle timer if configured
                if (_config.IdleTimeoutMinutes > 0)
                {
                    SetupIdleTimer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize browser: {ex.Message}",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadBlockedDomains()
        {
            if (!string.IsNullOrWhiteSpace(_config.BlockedDomains))
            {
                var domains = _config.BlockedDomains.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var domain in domains)
                {
                    _blockedDomains.Add(domain.Trim().ToLower());
                }
            }
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Check if domain is blocked
            try
            {
                var uri = new Uri(e.Uri);
                if (_blockedDomains.Any(blocked => uri.Host.Contains(blocked, StringComparison.OrdinalIgnoreCase)))
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        "This website is not available in kiosk mode.",
                        "Access Restricted",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }
            catch { }

            // Update URL text box
            UrlTextBox.Text = e.Uri;
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            Dispatcher.Invoke(() =>
            {
                PageTitleText.Text = WebBrowser.CoreWebView2.DocumentTitle;
                Title = WebBrowser.CoreWebView2.DocumentTitle + " - Kiosk Browser";
            });
        }

        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            Dispatcher.Invoke(() =>
            {
                BackButton.IsEnabled = WebBrowser.CanGoBack;
                ForwardButton.IsEnabled = WebBrowser.CanGoForward;
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebBrowser.CanGoForward)
            {
                WebBrowser.GoForward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser.Reload();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_config.HomePage))
            {
                WebBrowser.Source = new Uri(_config.HomePage);
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl(UrlTextBox.Text);
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                NavigateToUrl(UrlTextBox.Text);
                e.Handled = true;
            }
        }

        private void UrlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UrlTextBox.SelectAll();
        }

        private void NavigateToUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return;

                // If it doesn't look like a URL, treat it as a search query
                if (!url.Contains("://") && !url.Contains("."))
                {
                    url = $"https://www.google.com/search?q={Uri.EscapeDataString(url)}";
                }
                else if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                WebBrowser.Source = new Uri(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Invalid URL: {ex.Message}",
                    "Navigation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleVirtualKeyboard();
        }

        private void ToggleVirtualKeyboard()
        {
            if (VirtualKeyboardPanel.Visibility == Visibility.Visible)
            {
                VirtualKeyboardPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                VirtualKeyboardPanel.Visibility = Visibility.Visible;
                // Focus the URL textbox when keyboard is shown
                if (_adminMode && UrlPanel.Visibility == Visibility.Visible)
                {
                    UrlTextBox.Focus();
                }
            }
        }

        private void CloseKeyboard_Click(object sender, RoutedEventArgs e)
        {
            VirtualKeyboardPanel.Visibility = Visibility.Collapsed;
        }

        private void VirtualKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string key)
            {
                if (UrlTextBox.IsFocused)
                {
                    int caretIndex = UrlTextBox.CaretIndex;
                    UrlTextBox.Text = UrlTextBox.Text.Insert(caretIndex, key);
                    UrlTextBox.CaretIndex = caretIndex + key.Length;
                }
                else
                {
                    // Send key to WebView2
                    SendKeysToWebView(key);
                }
            }
        }

        private void VirtualBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (UrlTextBox.IsFocused && UrlTextBox.Text.Length > 0 && UrlTextBox.CaretIndex > 0)
            {
                int caretIndex = UrlTextBox.CaretIndex;
                UrlTextBox.Text = UrlTextBox.Text.Remove(caretIndex - 1, 1);
                UrlTextBox.CaretIndex = caretIndex - 1;
            }
        }

        private async void SendKeysToWebView(string text)
        {
            // Inject text into focused element in WebView2
            await WebBrowser.CoreWebView2.ExecuteScriptAsync(
                $"document.activeElement.value += '{text.Replace("'", "\\'")}';");
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WebBrowser.CoreWebView2.ShowPrintUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Print failed: {ex.Message}",
                    "Print Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }

        private void ToggleFullscreen()
        {
            if (_isFullscreen)
            {
                ExitFullscreen();
            }
            else
            {
                EnterFullscreen();
            }
        }

        private void EnterFullscreen()
        {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            _isFullscreen = true;
        }

        private void ExitFullscreen()
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.CanResize;
            Topmost = false;
            _isFullscreen = false;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_config);
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {
                // Reload configuration
                MessageBox.Show(
                    "Settings saved. Please restart the application for changes to take effect.",
                    "Settings",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to exit kiosk mode?",
                "Exit Kiosk Browser",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Admin mode toggle (Press Ctrl+Alt+Shift+A five times quickly)
            if (e.Key == Key.A &&
                Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift))
            {
                _adminKeyPressCount++;

                if (_adminKeyTimer == null)
                {
                    _adminKeyTimer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(3)
                    };
                    _adminKeyTimer.Tick += (s, args) =>
                    {
                        _adminKeyPressCount = 0;
                        _adminKeyTimer?.Stop();
                    };
                }

                _adminKeyTimer.Stop();
                _adminKeyTimer.Start();

                if (_adminKeyPressCount >= 5)
                {
                    ToggleAdminMode();
                    _adminKeyPressCount = 0;
                    _adminKeyTimer.Stop();
                }
                e.Handled = true;
                return;
            }

            // Admin mode shortcuts
            if (_adminMode)
            {
                if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    SettingsButton_Click(sender, e);
                    e.Handled = true;
                    return;
                }
                if (e.Key == Key.X && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    ExitButton_Click(sender, e);
                    e.Handled = true;
                    return;
                }
            }

            // Standard shortcuts
            switch (e.Key)
            {
                case Key.F2:
                    ToggleVirtualKeyboard();
                    e.Handled = true;
                    break;

                case Key.F5:
                    RefreshButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.F11:
                    ToggleFullscreen();
                    e.Handled = true;
                    break;

                case Key.Home when Keyboard.Modifiers == ModifierKeys.Alt:
                    HomeButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.Left when Keyboard.Modifiers == ModifierKeys.Alt:
                    BackButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.Right when Keyboard.Modifiers == ModifierKeys.Alt:
                    ForwardButton_Click(sender, e);
                    e.Handled = true;
                    break;

                case Key.P when Keyboard.Modifiers == ModifierKeys.Control:
                    PrintButton_Click(sender, e);
                    e.Handled = true;
                    break;
            }
        }

        private void ToggleAdminMode()
        {
            _adminMode = !_adminMode;

            if (_adminMode)
            {
                ExitButton.Visibility = Visibility.Visible;
                SettingsButton.Visibility = Visibility.Visible;
                UrlPanel.Visibility = Visibility.Visible;
                PageTitleText.Visibility = Visibility.Collapsed;

                MessageBox.Show(
                    "Admin mode enabled.\n\nCtrl+Alt+S: Settings\nCtrl+Alt+X: Exit",
                    "Admin Mode",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                ExitButton.Visibility = Visibility.Collapsed;
                SettingsButton.Visibility = Visibility.Collapsed;
                UrlPanel.Visibility = Visibility.Collapsed;
                PageTitleText.Visibility = Visibility.Visible;
            }
        }

        private void WebBrowser_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Visible;
            StartLoadingAnimation();
        }

        private void WebBrowser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            LoadingPanel.Visibility = Visibility.Collapsed;
            StopLoadingAnimation();
        }

        private Storyboard? _loadingStoryboard;

        private void StartLoadingAnimation()
        {
            var rotateTransform = (RotateTransform)LoadingSpinner.RenderTransform;
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };

            _loadingStoryboard = new Storyboard();
            _loadingStoryboard.Children.Add(animation);
            Storyboard.SetTarget(animation, rotateTransform);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RotateTransform.AngleProperty));
            _loadingStoryboard.Begin();
        }

        private void StopLoadingAnimation()
        {
            _loadingStoryboard?.Stop();
        }

        private System.Windows.Threading.DispatcherTimer? _idleTimer;
        private DateTime _lastActivity;

        private void SetupIdleTimer()
        {
            _lastActivity = DateTime.Now;

            _idleTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _idleTimer.Tick += (s, e) =>
            {
                var idleTime = DateTime.Now - _lastActivity;
                if (idleTime.TotalMinutes >= _config.IdleTimeoutMinutes)
                {
                    // Reset to home page
                    HomeButton_Click(this, new RoutedEventArgs());
                    _lastActivity = DateTime.Now;
                }
            };
            _idleTimer.Start();

            // Track user activity
            MouseMove += (s, e) => _lastActivity = DateTime.Now;
            KeyDown += (s, e) => _lastActivity = DateTime.Now;
        }
    }
}
