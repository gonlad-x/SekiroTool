using System.Collections.ObjectModel;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly HotkeyManager _hotkeyManager;
    private readonly Dictionary<string, StartupOptionViewModel> _lookup;
    private bool _startupApplied;

    public ObservableCollection<StartupOptionViewModel> PlayerOptions { get; }
    public ObservableCollection<StartupOptionViewModel> EnemiesOptions { get; }
    public ObservableCollection<StartupOptionViewModel> TargetOptions { get; }
    public ObservableCollection<StartupOptionViewModel> UtilityOptions { get; }
    public ObservableCollection<StartupOptionViewModel> BossSkipOptions { get; }

    public StartupViewModel(HotkeyManager hotkeyManager, IStateService stateService)
    {
        _hotkeyManager = hotkeyManager;

        PlayerOptions =
        [
            new("Save Position 1", HotkeyActions.SavePos1),
            new("Save Position 2", HotkeyActions.SavePos2),
            new("Restore Position 1", HotkeyActions.RestorePos1),
            new("Restore Position 2", HotkeyActions.RestorePos2),
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
            new("Increase Player Speed", HotkeyActions.IncreasePlayerSpeed),
            new("Decrease Player Speed", HotkeyActions.DecreasePlayerSpeed),
            new("Toggle Player Speed", HotkeyActions.TogglePlayerSpeed),
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
            new("All No Death", HotkeyActions.AllNoDeath),
            new("All No Damage", HotkeyActions.AllNoDamage),
            new("All No Hit", HotkeyActions.AllNoHit),
            new("All No Attack", HotkeyActions.AllNoAttack),
            new("All No Move", HotkeyActions.AllNoMove),
            new("All Disable AI", HotkeyActions.AllDisableAi),
            new("All No Posture Buildup", HotkeyActions.AllNoPostureBuildup),
            new("All Targeting View", HotkeyActions.AllTargetingView),
        ];

        TargetOptions =
        [
            new("Enable Target Options", HotkeyActions.EnableTargetOptions),
            new("Freeze Target HP", HotkeyActions.FreezeTargetHp),
            new("Set Target One HP", HotkeyActions.SetTargetOneHp),
            new("Target Custom HP", HotkeyActions.TargetCustomHp),
            new("Freeze Target Posture", HotkeyActions.FreezeTargetPosture),
            new("Set Target One Posture", HotkeyActions.SetTargetOnePosture),
            new("Target Custom Posture", HotkeyActions.TargetCustomPosture),
            new("Show All Resistances", HotkeyActions.ShowAllResistances),
            new("Repeat Act", HotkeyActions.RepeatAct),
            new("Repeat Kengeki Act", HotkeyActions.RepeatKengekiAct),
            new("Increment Force Act", HotkeyActions.IncrementForceAct),
            new("Decrement Force Act", HotkeyActions.DecrementForceAct),
            new("Increment Force Kengeki Act", HotkeyActions.IncrementForceKengekiAct),
            new("Decrement Force Kengeki Act", HotkeyActions.DecrementForceKengekiAct),
            new("Increase Target Speed", HotkeyActions.IncreaseTargetSpeed),
            new("Decrease Target Speed", HotkeyActions.DecreaseTargetSpeed),
            new("Toggle Target Speed", HotkeyActions.ToggleTargetSpeed),
            new("Freeze Target AI", HotkeyActions.FreezeTargetAi),
            new("No Attack Target AI", HotkeyActions.NoAttackTargetAi),
            new("No Move Target AI", HotkeyActions.NoMoveTargetAi),
            new("Target No Posture Buildup", HotkeyActions.TargetNoPostureBuildup),
            new("Target No Death", HotkeyActions.TargetNoDeath),
            new("Target Targeting View", HotkeyActions.TargetTargetingView),
            new("Toggle Target Overlay", HotkeyActions.ToggleTargetOverlay),
            new("Reset Hit Count", HotkeyActions.ResetHitCount),
        ];

        UtilityOptions =
        [
            new("Quitout", HotkeyActions.Quitout),
            new("Toggle Game Speed", HotkeyActions.ToggleGameSpeed),
            new("Increase Game Speed", HotkeyActions.IncreaseGameSpeed),
            new("Decrease Game Speed", HotkeyActions.DecreaseGameSpeed),
            new("No Clip", HotkeyActions.NoClip),
            new("Increase No Clip Speed", HotkeyActions.IncreaseNoClipSpeed),
            new("Decrease No Clip Speed", HotkeyActions.DecreaseNoClipSpeed),
            new("Free Cam", HotkeyActions.FreeCam),
            new("Move Cam To Player", HotkeyActions.MoveCamToPlayer),
        ];

        BossSkipOptions =
        [
            new("Skip Geni 3", HotkeyActions.Geni3Skip),
            new("Skip Geni 2 (Armor)", HotkeyActions.Geni2Skip),
            new("Skip Emma", HotkeyActions.EmmaSkip),
        ];

        _lookup = PlayerOptions
            .Concat(EnemiesOptions)
            .Concat(TargetOptions)
            .Concat(UtilityOptions)
            .Concat(BossSkipOptions)
            .ToDictionary(o => o.ActionId);

        foreach (var option in _lookup.Values)
            option.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(StartupOptionViewModel.IsEnabled))
                {
                    Save();
                    OnPropertyChanged(nameof(Summary));
                }
            };

        Load();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.Detached, () => _startupApplied = false);
    }

    public string Summary
    {
        get
        {
            var enabled = _lookup.Values.Where(o => o.IsEnabled).Select(o => o.DisplayName).ToList();
            return enabled.Count == 0 ? "None" : string.Join(", ", enabled);
        }
    }

    private void OnGameLoaded()
    {
        if (_startupApplied) return;
        _startupApplied = true;
        foreach (var option in _lookup.Values.Where(o => o.IsEnabled))
            _hotkeyManager.TriggerAction(option.ActionId);
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
    }
}
