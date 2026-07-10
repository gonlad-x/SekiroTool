using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class StartupTab : UserControl
{
    public StartupTab(StartupViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
