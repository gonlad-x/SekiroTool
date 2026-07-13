using System.Collections.ObjectModel;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly HotkeyManager _hotkeyManager;
    private readonly Dictionary<string, StartupOptionViewModel> _gameLoadOptions;
    private readonly Dictionary<string, StartupOptionViewModel> _newGameOptions;
    private readonly Dictionary<string, StartupOptionViewModel> _lookup;
    private bool _startupApplied;
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (!SetProperty(ref _isEnabled, value)) return;
            SettingsManager.Default.StartupOptionsEnabled = value;
            SettingsManager.Default.Save();
        }
    }

    // Activate on Game Load
    public ObservableCollection<StartupOptionViewModel> PlayerOptions { get; }
    public ObservableCollection<StartupOptionViewModel> EnemiesOptions { get; }
    public ObservableCollection<StartupOptionViewModel> TargetOptions { get; }
    public ObservableCollection<StartupOptionViewModel> BossSkipOptions { get; }

    // Activate on New Game Cycle
    public ObservableCollection<StartupOptionViewModel> NewGamePlayerOptions { get; }
    public ObservableCollection<StartupOptionViewModel> EventOptions { get; }

    public StartupViewModel(HotkeyManager hotkeyManager, IStateService stateService)
    {
        _hotkeyManager = hotkeyManager;

        PlayerOptions =
        [
            new("Apply Confetti", HotkeyActions.ApplyConfetti),
            new("Apply Gachiin", HotkeyActions.ApplyGachiin),
            new("Remove Confetti", HotkeyActions.RemoveConfetti),
            new("Remove Gachiin", HotkeyActions.RemoveGachiin),
            new("One Shot Health", HotkeyActions.OneShotHealth),
            new("One Shot Posture", HotkeyActions.OneShotPosture),
            new("No Goods Consume", HotkeyActions.NoGoodsConsume),
            new("No Emblem Consume", HotkeyActions.NoEmblemConsume),
            new("Infinite Revival", HotkeyActions.InfiniteRevival),
            new("Player Hide", HotkeyActions.PlayerHide),
            new("Player Silent", HotkeyActions.PlayerSilent),
            new("Infinite Poise", HotkeyActions.InfinitePoise),
            new("No Death", HotkeyActions.NoDeath),
            new("No Death (Yes Killbox)", HotkeyActions.NoDeathExKillbox),
            new("No Damage", HotkeyActions.NoDamage),
            new("Increase Damage Multiplier", HotkeyActions.IncreaseDamageMultiplier),
            new("Decrease Damage Multiplier", HotkeyActions.DecreaseDamageMultiplier),
            new("Toggle Damage Multiplier", HotkeyActions.ToggleDamageMultiplier),
        ];

        EnemiesOptions =
        [
            new("Skip Dragon Phase One", HotkeyActions.SkipDragonPhaseOne),
            new("Trigger Dragon Final Attack", HotkeyActions.TriggerDragonFinalAttack),
            new("No Butterfly Summons", HotkeyActions.NoButterflySummons),
            new("Snake Intro Loop", HotkeyActions.SnakeIntroLoop),
        ];

        TargetOptions =
        [
            new("Enable Target Options", HotkeyActions.EnableTargetOptions),
            new("Pop Out Overlay", HotkeyActions.ToggleTargetOverlay),
        ];

        BossSkipOptions =
        [
            new("Skip Geni 3", HotkeyActions.Geni3Skip),
            new("Skip Geni 2 (Armor)", HotkeyActions.Geni2Skip),
            new("Skip Emma", HotkeyActions.EmmaSkip),
        ];

        NewGamePlayerOptions =
        [
            new("Max HP", HotkeyActions.SetMaxHp),
            new("Auto Set NG+7", HotkeyActions.SetNewGame7),
        ];

        EventOptions =
        [
            new("Demon Bell On", HotkeyActions.DemonBellOn),
            new("Demon Bell Off", HotkeyActions.DemonBellOff),
            new("No Kuro's Charm On", HotkeyActions.NoKurosCharmOn),
            new("No Kuro's Charm Off", HotkeyActions.NoKurosCharmOff),
        ];

        _gameLoadOptions = PlayerOptions
            .Concat(EnemiesOptions)
            .Concat(TargetOptions)
            .Concat(BossSkipOptions)
            .ToDictionary(o => o.ActionId);

        _newGameOptions = NewGamePlayerOptions
            .Concat(EventOptions)
            .ToDictionary(o => o.ActionId);

        _lookup = _gameLoadOptions
            .Concat(_newGameOptions)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var option in _lookup.Values)
            option.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(StartupOptionViewModel.IsEnabled))
                {
                    Save();
                    OnPropertyChanged(nameof(Summary));
                    OnPropertyChanged(nameof(NewGameSummary));
                }
            };

        Load();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.GameStart, OnGameStart);
        stateService.Subscribe(State.Detached, () => _startupApplied = false);
    }

    public string Summary
    {
        get
        {
            var enabled = _gameLoadOptions.Values.Where(o => o.IsEnabled).Select(o => o.DisplayName).ToList();
            return enabled.Count == 0 ? "None" : string.Join(", ", enabled);
        }
    }

    public string NewGameSummary
    {
        get
        {
            var enabled = _newGameOptions.Values.Where(o => o.IsEnabled).Select(o => o.DisplayName).ToList();
            return enabled.Count == 0 ? "None" : string.Join(", ", enabled);
        }
    }

    private void OnGameLoaded()
    {
        if (_startupApplied) return;
        _startupApplied = true;
        if (!IsEnabled) return;
        foreach (var option in _gameLoadOptions.Values.Where(o => o.IsEnabled))
            _hotkeyManager.TriggerStartupAction(option.ActionId);
    }

    private void OnGameStart()
    {
        if (!IsEnabled) return;

        // Also silently reasserts enabled "Activate on Game Load" options whose
        // underlying effect lives in per-character memory (e.g. No Damage) and
        // would otherwise be lost when a New Game allocates a fresh character.
        foreach (var option in _lookup.Values.Where(o => o.IsEnabled))
            _hotkeyManager.TriggerNewGameAction(option.ActionId);
    }

    private void Save()
    {
        var enabled = _lookup.Values.Where(o => o.IsEnabled).Select(o => o.ActionId);
        SettingsManager.Default.StartupActionIds = string.Join(";", enabled);
        SettingsManager.Default.Save();
    }

    private void Load()
    {
        var ids = new HashSet<string>(
            SettingsManager.Default.StartupActionIds
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

        foreach (var option in _lookup.Values)
            option.IsEnabled = ids.Contains(option.ActionId);

        _isEnabled = SettingsManager.Default.StartupOptionsEnabled;
    }
}
