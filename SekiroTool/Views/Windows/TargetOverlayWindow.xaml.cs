using System.Windows;
using System.Windows.Media;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Windows;

public partial class TargetOverlayWindow : Window
{
    private double _scaleMultiplier = 1.0;
    private double _backgroundOpacity = 0.85;

    public TargetOverlayWindow()
    {
        InitializeComponent();

        MouseLeftButtonDown += (_, _) => DragMove();

        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.Closing += (_, _) => Close();

        Loaded += (_, _) =>
        {
            if (SettingsManager.Default.TargetOverlayScaleX > 0)
            {
                _scaleMultiplier = SettingsManager.Default.TargetOverlayScaleX;
                ContentScale.ScaleX = _scaleMultiplier;
                ContentScale.ScaleY = _scaleMultiplier;
            }

            if (SettingsManager.Default.TargetOverlayOpacity > 0)
                _backgroundOpacity = SettingsManager.Default.TargetOverlayOpacity;

            UpdateBackground();
        };

        ContentRendered += (_, _) =>
        {
            if (!double.IsNaN(SettingsManager.Default.TargetOverlayLeft) &&
                !double.IsNaN(SettingsManager.Default.TargetOverlayTop))
            {
                Left = SettingsManager.Default.TargetOverlayLeft;
                Top = SettingsManager.Default.TargetOverlayTop;
            }
            else
            {
                var area = SystemParameters.WorkArea;
                Left = area.Right - ActualWidth - 10;
                Top = area.Top + 10;
            }
        };
    }

    private void UpdateBackground()
    {
        MainBorder.Background = new SolidColorBrush(
            Color.FromArgb((byte)(_backgroundOpacity * 255), 0x1E, 0x1E, 0x1E));
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void CycleOpacity_Click(object sender, RoutedEventArgs e)
    {
        _backgroundOpacity = _backgroundOpacity >= 0.9 ? 0.3 : _backgroundOpacity + 0.2;
        UpdateBackground();
    }

    private void DecreaseSize_Click(object sender, RoutedEventArgs e)
    {
        if (_scaleMultiplier > 0.6)
        {
            _scaleMultiplier -= 0.2;
            ContentScale.ScaleX = _scaleMultiplier;
            ContentScale.ScaleY = _scaleMultiplier;
        }
    }

    private void IncreaseSize_Click(object sender, RoutedEventArgs e)
    {
        if (_scaleMultiplier < 3.0)
        {
            _scaleMultiplier += 0.2;
            ContentScale.ScaleX = _scaleMultiplier;
            ContentScale.ScaleY = _scaleMultiplier;
        }
    }

    private void ToggleDetailedView_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TargetViewModel vm)
            vm.IsOverlayDetailedViewEnabled = !vm.IsOverlayDetailedViewEnabled;
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);

        SettingsManager.Default.TargetOverlayScaleX = _scaleMultiplier;
        SettingsManager.Default.TargetOverlayOpacity = _backgroundOpacity;
        SettingsManager.Default.TargetOverlayLeft = Left;
        SettingsManager.Default.TargetOverlayTop = Top;
        SettingsManager.Default.Save();
    }
}
