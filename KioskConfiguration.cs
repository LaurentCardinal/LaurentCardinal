using System;
using System.IO;
using System.Text.Json;

namespace KioskBrowser
{
    public class KioskConfiguration
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "KioskBrowser",
            "config.json");

        public string HomePage { get; set; } = "https://www.google.com";
        public bool StartFullscreen { get; set; } = true;
        public bool ShowVirtualKeyboard { get; set; } = true;
        public bool EnableContextMenu { get; set; } = false;
        public bool EnableZoom { get; set; } = true;
        public int IdleTimeoutMinutes { get; set; } = 5;
        public string BlockedDomains { get; set; } = "";
        public bool EnablePrinting { get; set; } = true;
        public bool AllowDownloads { get; set; } = false;
        public string DownloadPath { get; set; } = "";

        public static KioskConfiguration Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<KioskConfiguration>(json) ?? new KioskConfiguration();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to load configuration: {ex.Message}\nUsing default settings.",
                    "Configuration Warning",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }

            return new KioskConfiguration();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(ConfigPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to save configuration: {ex.Message}",
                    "Configuration Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public static string GetConfigPath()
        {
            return ConfigPath;
        }
    }
}
