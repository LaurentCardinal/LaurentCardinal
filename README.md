# Kiosk Browser

A modern, reliable web browser designed specifically for public Internet kiosks, built with C# WPF and Microsoft Edge WebView2.

## Features

### Beautiful & Accessible UI
- Modern, clean interface with intuitive design
- High contrast, large touch-friendly buttons
- Accessible to users of all technical levels
- Smooth animations and visual feedback

### Security & Control
- Full-screen mode by default (no address bar visible)
- Domain blocking to restrict access to specific websites
- Configurable idle timeout with automatic reset to homepage
- Admin mode protection (requires specific key combination)
- Optional context menu and zoom controls

### Touch-Friendly
- Built-in virtual keyboard for touchscreen devices
- Large, easy-to-tap navigation buttons
- Optimized for public kiosk environments

### Powerful Browser Engine
- Microsoft Edge WebView2 for modern web standards
- Fast, secure, and reliable browsing
- Full HTML5, CSS3, and JavaScript support
- Regular security updates from Microsoft

## System Requirements

- **Operating System**: Windows 10/11 (64-bit)
- **.NET Runtime**: .NET 8.0 or later
- **WebView2 Runtime**: Microsoft Edge WebView2 Runtime (usually pre-installed on Windows 10/11)
- **RAM**: 4 GB minimum, 8 GB recommended
- **Storage**: 100 MB for application

## Installation

### Option 1: Build from Source

1. **Install Prerequisites**
   - [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

2. **Clone the Repository**
   ```bash
   git clone https://github.com/LaurentCardinal/LaurentCardinal.git
   cd LaurentCardinal
   ```

3. **Build the Application**
   ```bash
   dotnet restore
   dotnet build --configuration Release
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

### Option 2: Publish Standalone Executable

Create a self-contained executable that doesn't require .NET to be installed:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in: `bin/Release/net8.0-windows/win-x64/publish/KioskBrowser.exe`

## Configuration

The application stores its configuration in:
```
%AppData%\KioskBrowser\config.json
```

### Settings

| Setting | Description | Default |
|---------|-------------|---------|
| HomePage | URL to load on startup and reset | https://www.google.com |
| StartFullscreen | Start in fullscreen mode | true |
| ShowVirtualKeyboard | Show virtual keyboard button | true |
| EnableContextMenu | Enable right-click menu | false |
| EnableZoom | Allow Ctrl +/- zoom | true |
| EnablePrinting | Allow printing pages | true |
| IdleTimeoutMinutes | Minutes before auto-reset (0=disabled) | 5 |
| BlockedDomains | Comma-separated list of blocked domains | "" |
| AllowDownloads | Allow file downloads | false |
| DownloadPath | Where to save downloads | "" |

### Example Configuration

```json
{
  "HomePage": "https://www.example.com",
  "StartFullscreen": true,
  "ShowVirtualKeyboard": true,
  "EnableContextMenu": false,
  "EnableZoom": true,
  "IdleTimeoutMinutes": 10,
  "BlockedDomains": "facebook.com,twitter.com,youtube.com",
  "EnablePrinting": true,
  "AllowDownloads": false,
  "DownloadPath": ""
}
```

## Usage

### For Users

#### Navigation
- **Back**: Click the ◄ button or press Alt+Left
- **Forward**: Click the ► button or press Alt+Right
- **Refresh**: Click the ↻ button or press F5
- **Home**: Click the ⌂ button or press Alt+Home

#### Virtual Keyboard
- **Toggle**: Click the ⌨ button or press F2
- Use the on-screen keyboard to type in web forms
- Works with touchscreen devices

#### Other Features
- **Print**: Click the 🖨 button or press Ctrl+P
- **Fullscreen**: Press F11 to toggle fullscreen mode

### For Administrators

#### Accessing Admin Mode

Press **Ctrl+Alt+Shift+A** five times quickly (within 3 seconds) to enable admin mode.

When admin mode is enabled:
- Address bar becomes visible
- Settings button (⚙) appears
- Exit button (✕) appears

#### Admin Shortcuts
- **Ctrl+Alt+S**: Open settings
- **Ctrl+Alt+X**: Exit application
- **Ctrl+Alt+Shift+A** (5x): Toggle admin mode

#### Configuring Settings

1. Enable admin mode
2. Click the settings button (⚙) or press Ctrl+Alt+S
3. Modify settings as needed
4. Click "Save Settings"
5. Restart the application for changes to take effect

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| F2 | Toggle virtual keyboard |
| F5 | Refresh page |
| F11 | Toggle fullscreen |
| Alt+Left | Go back |
| Alt+Right | Go forward |
| Alt+Home | Go to homepage |
| Ctrl+P | Print page |
| Ctrl+Alt+S | Settings (admin mode) |
| Ctrl+Alt+X | Exit (admin mode) |
| Ctrl+Alt+Shift+A (5x) | Toggle admin mode |

## Kiosk Mode Setup

### Windows Kiosk Mode (Shell Launcher)

For a true kiosk experience, configure Windows to automatically launch the browser on startup:

1. **Create a Startup Shortcut**
   - Press Win+R and type `shell:startup`
   - Create a shortcut to `KioskBrowser.exe`

2. **Use Windows Kiosk Mode** (Windows 10/11 Pro)
   - Enable Assigned Access in Settings
   - Assign a kiosk user account
   - Set KioskBrowser as the kiosk application

3. **Group Policy Configuration** (Enterprise)
   - Configure user shell replacement via Group Policy
   - Set KioskBrowser.exe as the custom shell

### Auto-Start on Login

Create a batch file `StartKiosk.bat`:

```batch
@echo off
start "" "C:\Path\To\KioskBrowser.exe"
```

Place in: `%AppData%\Microsoft\Windows\Start Menu\Programs\Startup`

## Security Considerations

### Recommended Settings for Public Kiosks

```json
{
  "EnableContextMenu": false,
  "AllowDownloads": false,
  "IdleTimeoutMinutes": 5,
  "BlockedDomains": "facebook.com,twitter.com,instagram.com"
}
```

### Additional Security Measures

1. **Use a restricted Windows account** with no admin privileges
2. **Enable Windows Update** to keep WebView2 current
3. **Configure firewall rules** if needed
4. **Use content filtering** at the network level
5. **Regular monitoring** of usage and logs
6. **Physical security** - secure the device hardware

## Troubleshooting

### WebView2 Not Found

If you get an error about WebView2 Runtime:

1. Download and install [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/)
2. Restart the application

### Application Won't Start

- Ensure .NET 8.0 Runtime is installed
- Check Windows Event Viewer for error details
- Try running as administrator (one time only)

### Can't Exit Fullscreen

- Press F11 to toggle fullscreen mode
- Enable admin mode and click the exit button

### Virtual Keyboard Not Working

- Ensure the keyboard button is visible in settings
- Press F2 to toggle the virtual keyboard
- Check that ShowVirtualKeyboard is set to true in config

### Website Blocked

If a legitimate site is blocked:
1. Enable admin mode
2. Open settings
3. Remove the domain from BlockedDomains
4. Save and restart

## Development

### Project Structure

```
KioskBrowser/
├── KioskBrowser.csproj       # Project file
├── App.xaml                  # Application resources & styles
├── App.xaml.cs               # Application entry point
├── MainWindow.xaml           # Main window UI
├── MainWindow.xaml.cs        # Main window logic
├── SettingsWindow.xaml       # Settings dialog UI
├── SettingsWindow.xaml.cs    # Settings dialog logic
├── KioskConfiguration.cs     # Configuration management
└── README.md                 # This file
```

### Technologies Used

- **C# 12** - Programming language
- **WPF** - Windows Presentation Foundation UI framework
- **WebView2** - Microsoft Edge browser engine
- **.NET 8.0** - Runtime framework
- **XAML** - UI markup language

### Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source and available for use in public kiosks, libraries, schools, and other institutional settings.

## Support

For issues, questions, or feature requests, please open an issue on GitHub.

## Roadmap

Future enhancements planned:

- [ ] Multi-language support (i18n)
- [ ] Session time limits with warning dialogs
- [ ] Usage analytics and reporting
- [ ] Customizable keyboard layouts
- [ ] Whitelist mode (allow only specific domains)
- [ ] Remote management and monitoring
- [ ] Printer quota management
- [ ] Automatic screenshot blocking
- [ ] Integration with payment systems

## Acknowledgments

- Built with [Microsoft Edge WebView2](https://developer.microsoft.com/microsoft-edge/webview2/)
- UI inspired by modern web design principles
- Designed for accessibility and ease of use

---

**Made with ❤️ for public Internet access**
