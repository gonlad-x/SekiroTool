using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SekiroTool.Utilities
{
    public static class VersionChecker
    {
        private const string Repo = "gonlad-x/SekiroTool";

        public static async Task<(bool hasUpdate, Version currentVersion, Version webVersion)> CheckForUpdate()
        {
            try
            {
                var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                if (currentVersion == null) return (false, null, null);

                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("SekiroTool", currentVersion.ToString()));

                var response = await client.GetStringAsync(
                    $"https://api.github.com/repos/{Repo}/releases/latest");

                int tagIndex = response.IndexOf("\"tag_name\":", StringComparison.OrdinalIgnoreCase);
                if (tagIndex == -1) return (false, currentVersion, null);

                int quoteStart = response.IndexOf('"', tagIndex + "\"tag_name\":".Length) + 1;
                int quoteEnd = response.IndexOf('"', quoteStart);

                if (quoteStart == -1 || quoteEnd == -1) return (false, currentVersion, null);

                var webVersion = new Version(response.Substring(quoteStart, quoteEnd - quoteStart).TrimStart('v'));

                return (webVersion > currentVersion, currentVersion, webVersion);
            }
            catch
            {
                return (false, null, null);
            }
        }

        public static async void CheckForUpdates(Window parentWindow, bool showNoUpdateMessage = false)
        {
            var (hasUpdate, currentVersion, webVersion) = await CheckForUpdate();

            if (!hasUpdate || webVersion == null || currentVersion == null)
            {
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        "Your application is up to date.",
                        "Update Check",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                return;
            }

            var updateWindow = new Window
            {
                Title = "Update Available",
                Width = 300,
                Height = 200,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = parentWindow,
                Background = (SolidColorBrush)Application.Current.Resources["BackgroundBrush"],
                BorderBrush = (SolidColorBrush)Application.Current.Resources["BorderBrush"],
                BorderThickness = new Thickness(1)
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

            var titleBar = new Border
            {
                Background = (SolidColorBrush)Application.Current.Resources["TitleBarBrush"],
                Child = new TextBlock
                {
                    Text = "Update Available",
                    Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                    Margin = new Thickness(10, 5, 0, 0)
                }
            };
            Grid.SetRow(titleBar, 0);
            grid.Children.Add(titleBar);

            var message = new TextBlock
            {
                Text = $"A new version (v{webVersion.Major}.{webVersion.Minor}.{webVersion.Build}) is available!\nCurrent version: v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}",
                Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20, 10, 20, 10),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(message, 1);
            grid.Children.Add(message);

            var dontShowCheckbox = new CheckBox
            {
                Content = "Don't show on app launch",
                Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                Margin = new Thickness(20, 5, 20, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                IsChecked = !SettingsManager.Default.EnableUpdateChecks
            };
            Grid.SetRow(dontShowCheckbox, 1);
            grid.Children.Add(dontShowCheckbox);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var updateButton = new Button
            {
                Content = "Update",
                Width = 80,
                Height = 25,
                Margin = new Thickness(5)
            };

            updateButton.Click += async (s, e) =>
            {
                updateButton.IsEnabled = false;
                updateButton.Content = "Downloading...";
                SettingsManager.Default.EnableUpdateChecks = dontShowCheckbox.IsChecked != true;
                SettingsManager.Default.Save();

                try
                {
                    var exePath = Process.GetCurrentProcess().MainModule!.FileName;
                    var dir = Path.GetDirectoryName(exePath)!;
                    var updatePath = Path.Combine(dir, "SekiroTool_update.exe");

                    var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
                    client.DefaultRequestHeaders.UserAgent.Add(
                        new ProductInfoHeaderValue("SekiroTool", currentVersion.ToString()));
                    var bytes = await client.GetByteArrayAsync(
                        $"https://github.com/{Repo}/releases/latest/download/SekiroTool.exe");
                    File.WriteAllBytes(updatePath, bytes);

                    var batPath = Path.Combine(dir, "SekiroTool_update.bat");
                    File.WriteAllText(batPath,
                        $"@echo off\r\ntimeout /t 2 /nobreak >nul\r\nmove /y \"{updatePath}\" \"{exePath}\"\r\nstart \"\" \"{exePath}\"\r\ndel \"%~f0\"");

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c \"{batPath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });

                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    updateButton.IsEnabled = true;
                    updateButton.Content = "Update";
                    MessageBox.Show($"Update failed: {ex.Message}", "Update Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            var laterButton = new Button
            {
                Content = "Later",
                Width = 80,
                Height = 25,
                Margin = new Thickness(5)
            };

            laterButton.Click += (s, e) =>
            {
                SettingsManager.Default.EnableUpdateChecks = dontShowCheckbox.IsChecked != true;
                SettingsManager.Default.Save();
                updateWindow.Close();
            };

            buttonPanel.Children.Add(updateButton);
            buttonPanel.Children.Add(laterButton);
            grid.Children.Add(buttonPanel);
            Grid.SetRow(buttonPanel, 2);

            updateWindow.Content = grid;
            titleBar.MouseLeftButtonDown += (s, e) => updateWindow.DragMove();
            updateWindow.ShowDialog();
        }

        public static void UpdateVersionText(TextBlock appVersion)
        {
            var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;
            if (currentVersion != null)
                appVersion.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
        }
    }
}
