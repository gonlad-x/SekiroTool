using SekiroTool.Enums;

namespace SekiroTool.ViewModels;

public class StartupOptionViewModel(string displayName, HotkeyActions action) : BaseViewModel
{
    public string DisplayName { get; } = displayName;
    public string ActionId { get; } = action.ToString();

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
