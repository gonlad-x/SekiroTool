using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;
using SekiroTool.Views.Windows;

namespace SekiroTool.Views.Tabs;

public partial class TargetTab : UserControl
{
    private readonly TargetViewModel _targetViewModel;
    private TargetOverlayWindow? _overlayWindow;

    public TargetTab(TargetViewModel targetViewModel)
    {
        _targetViewModel = targetViewModel;
        InitializeComponent();
        DataContext = targetViewModel;

        _targetViewModel.PropertyChanged += OnViewModelPropertyChanged;

        InitializeUpDownHelpers();
    }

    private void InitializeUpDownHelpers()
    {
        _ = new UpDownHelper<double>(
            SpeedUpDown,
            _targetViewModel.SetSpeed
        );
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TargetViewModel.IsOverlayOpen)) return;
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_targetViewModel.IsOverlayOpen) OpenOverlay();
            else CloseOverlay();
        });
    }

    private void OpenOverlay()
    {
        if (_overlayWindow != null) return;
        _overlayWindow = new TargetOverlayWindow();
        _overlayWindow.DataContext = _targetViewModel;
        _overlayWindow.Closed += (_, _) =>
        {
            _overlayWindow = null;
            _targetViewModel.IsOverlayOpen = false;
        };
        _overlayWindow.Show();
    }

    private void CloseOverlay()
    {
        _overlayWindow?.Close();
    }
}