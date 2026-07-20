using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SekiroTool.ViewModels;
using SekiroTool.Views.Windows;

namespace SekiroTool.Views.Tabs;

public partial class SettingsTab : UserControl
{
    private readonly SettingsViewModel _settingsViewModel;
    private readonly ActivateOnLaunchViewModel _activateOnLaunchViewModel;

    public SettingsTab(SettingsViewModel settingsViewModel, ActivateOnLaunchViewModel activateOnLaunchViewModel)
    {
        DataContext = settingsViewModel;
        _settingsViewModel = settingsViewModel;
        _activateOnLaunchViewModel = activateOnLaunchViewModel;
        InitializeComponent();
    }

    private void ActivateOnLaunchButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new ActivateOnLaunchWindow(_activateOnLaunchViewModel)
        {
            Owner = Window.GetWindow(this)
        };
        window.ShowDialog();
    }
    
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            Focus();
        }), System.Windows.Threading.DispatcherPriority.Input);
    }
    
    private void HotkeyTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            string actionId = textBox.Tag.ToString();
            _settingsViewModel.StartSettingHotkey(actionId);
        }
    }

    private void HotkeyTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        _settingsViewModel.ConfirmHotkey();
    }

    private void HotkeyTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _settingsViewModel.ConfirmHotkey();
            if (sender is TextBox textBox)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            _settingsViewModel.CancelSettingHotkey();
            if (sender is TextBox textBox)
            {
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            e.Handled = true;
        }
    }
}