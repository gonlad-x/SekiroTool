using System.Windows;
using System.Windows.Input;

namespace SekiroTool.Views.Windows;

public partial class TargetOverlayWindow : Window
{
    public TargetOverlayWindow()
    {
        InitializeComponent();

        if (Application.Current.MainWindow != null)
            Application.Current.MainWindow.Closing += (_, _) => Close();
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        var area = SystemParameters.WorkArea;
        Left = area.Right - ActualWidth - 10;
        Top = area.Top + 10;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
