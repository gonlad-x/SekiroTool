using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class ActivateOnLaunchViewModel : BaseViewModel
{
    private readonly PlayerViewModel _playerViewModel;
    private readonly EnemyViewModel _enemyViewModel;
    private readonly TargetViewModel _targetViewModel;
    private readonly EventViewModel _eventViewModel;
    private readonly ActivateOnLaunchManager _aol;

    public ActivateOnLaunchViewModel(PlayerViewModel playerViewModel, EnemyViewModel enemyViewModel,
        TargetViewModel targetViewModel, EventViewModel eventViewModel,
        ActivateOnLaunchManager activateOnLaunchManager, IStateService stateService)
    {
        _playerViewModel = playerViewModel;
        _enemyViewModel = enemyViewModel;
        _targetViewModel = targetViewModel;
        _eventViewModel = eventViewModel;
        _aol = activateOnLaunchManager;

        RegisterActions();

        stateService.Subscribe(State.AppStart, OnAppStart);
        stateService.Subscribe(State.Attached, OnGameAttached);
        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.GameStart, OnNewGameStart);
    }

    #region Properties

    // Master toggle
    private bool _isEnabled = SettingsManager.Default.ActivateOnLaunchEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (!SetProperty(ref _isEnabled, value)) return;
            SettingsManager.Default.ActivateOnLaunchEnabled = value;
            SettingsManager.Default.Save();
        }
    }

    // Helper macros
    private bool Get(string id) => _aol.GetBool(id);
    private void Set(string id, bool value) => _aol.SetBool(id, value);

    // Player
    private bool _isInfiniteConfettiChecked;

    public bool IsInfiniteConfettiChecked
    {
        get => _isInfiniteConfettiChecked;
        set
        {
            if (SetProperty(ref _isInfiniteConfettiChecked, value)) Set(nameof(IsInfiniteConfettiChecked), value);
        }
    }

    private bool _isInfiniteGachiinChecked;

    public bool IsInfiniteGachiinChecked
    {
        get => _isInfiniteGachiinChecked;
        set
        {
            if (SetProperty(ref _isInfiniteGachiinChecked, value)) Set(nameof(IsInfiniteGachiinChecked), value);
        }
    }

    private bool _isOneShotHealthChecked;

    public bool IsOneShotHealthChecked
    {
        get => _isOneShotHealthChecked;
        set
        {
            if (SetProperty(ref _isOneShotHealthChecked, value)) Set(nameof(IsOneShotHealthChecked), value);
        }
    }

    private bool _isOneShotPostureChecked;

    public bool IsOneShotPostureChecked
    {
        get => _isOneShotPostureChecked;
        set
        {
            if (SetProperty(ref _isOneShotPostureChecked, value)) Set(nameof(IsOneShotPostureChecked), value);
        }
    }

    private bool _isNoGoodsConsumeChecked;

    public bool IsNoGoodsConsumeChecked
    {
        get => _isNoGoodsConsumeChecked;
        set
        {
            if (SetProperty(ref _isNoGoodsConsumeChecked, value)) Set(nameof(IsNoGoodsConsumeChecked), value);
        }
    }

    private bool _isNoEmblemConsumeChecked;

    public bool IsNoEmblemConsumeChecked
    {
        get => _isNoEmblemConsumeChecked;
        set
        {
            if (SetProperty(ref _isNoEmblemConsumeChecked, value)) Set(nameof(IsNoEmblemConsumeChecked), value);
        }
    }

    private bool _isInfiniteRevivalChecked;

    public bool IsInfiniteRevivalChecked
    {
        get => _isInfiniteRevivalChecked;
        set
        {
            if (SetProperty(ref _isInfiniteRevivalChecked, value)) Set(nameof(IsInfiniteRevivalChecked), value);
        }
    }

    private bool _isPlayerHideChecked;

    public bool IsPlayerHideChecked
    {
        get => _isPlayerHideChecked;
        set
        {
            if (SetProperty(ref _isPlayerHideChecked, value)) Set(nameof(IsPlayerHideChecked), value);
        }
    }

    private bool _isPlayerSilentChecked;

    public bool IsPlayerSilentChecked
    {
        get => _isPlayerSilentChecked;
        set
        {
            if (SetProperty(ref _isPlayerSilentChecked, value)) Set(nameof(IsPlayerSilentChecked), value);
        }
    }

    private bool _isInfinitePoiseChecked;

    public bool IsInfinitePoiseChecked
    {
        get => _isInfinitePoiseChecked;
        set
        {
            if (SetProperty(ref _isInfinitePoiseChecked, value)) Set(nameof(IsInfinitePoiseChecked), value);
        }
    }

    private bool _isNoDeathChecked;

    public bool IsNoDeathChecked
    {
        get => _isNoDeathChecked;
        set
        {
            if (SetProperty(ref _isNoDeathChecked, value)) Set(nameof(IsNoDeathChecked), value);
        }
    }

    private bool _isNoDeathExKillboxChecked;

    public bool IsNoDeathExKillboxChecked
    {
        get => _isNoDeathExKillboxChecked;
        set
        {
            if (SetProperty(ref _isNoDeathExKillboxChecked, value)) Set(nameof(IsNoDeathExKillboxChecked), value);
        }
    }

    private bool _isNoDamageChecked;

    public bool IsNoDamageChecked
    {
        get => _isNoDamageChecked;
        set
        {
            if (SetProperty(ref _isNoDamageChecked, value)) Set(nameof(IsNoDamageChecked), value);
        }
    }

    private bool _isToggleDamageMultiplierChecked;

    public bool IsToggleDamageMultiplierChecked
    {
        get => _isToggleDamageMultiplierChecked;
        set
        {
            if (SetProperty(ref _isToggleDamageMultiplierChecked, value))
                Set(nameof(IsToggleDamageMultiplierChecked), value);
        }
    }

    private double _damageMultiplierValue;

    public double DamageMultiplierValue
    {
        get => _damageMultiplierValue;
        set
        {
            if (SetProperty(ref _damageMultiplierValue, value))
                _aol.SetDouble(nameof(DamageMultiplierValue), value);
        }
    }

    // Enemies
    private bool _isSkipDragonPhaseOneChecked;

    public bool IsSkipDragonPhaseOneChecked
    {
        get => _isSkipDragonPhaseOneChecked;
        set
        {
            if (SetProperty(ref _isSkipDragonPhaseOneChecked, value)) Set(nameof(IsSkipDragonPhaseOneChecked), value);
        }
    }

    private bool _isTriggerDragonFinalAttackChecked;

    public bool IsTriggerDragonFinalAttackChecked
    {
        get => _isTriggerDragonFinalAttackChecked;
        set
        {
            if (SetProperty(ref _isTriggerDragonFinalAttackChecked, value))
                Set(nameof(IsTriggerDragonFinalAttackChecked), value);
        }
    }

    private bool _isNoButterflySummonsChecked;

    public bool IsNoButterflySummonsChecked
    {
        get => _isNoButterflySummonsChecked;
        set
        {
            if (SetProperty(ref _isNoButterflySummonsChecked, value)) Set(nameof(IsNoButterflySummonsChecked), value);
        }
    }

    private bool _isSnakeIntroLoopChecked;

    public bool IsSnakeIntroLoopChecked
    {
        get => _isSnakeIntroLoopChecked;
        set
        {
            if (SetProperty(ref _isSnakeIntroLoopChecked, value)) Set(nameof(IsSnakeIntroLoopChecked), value);
        }
    }

    // Target
    private bool _isEnableTargetOptionsChecked;

    public bool IsEnableTargetOptionsChecked
    {
        get => _isEnableTargetOptionsChecked;
        set
        {
            if (SetProperty(ref _isEnableTargetOptionsChecked, value))
                Set(nameof(IsEnableTargetOptionsChecked), value);
        }
    }

    // Boss Skips
    private bool _isSkipGeni3Checked;

    public bool IsSkipGeni3Checked
    {
        get => _isSkipGeni3Checked;
        set
        {
            if (SetProperty(ref _isSkipGeni3Checked, value)) Set(nameof(IsSkipGeni3Checked), value);
        }
    }

    private bool _isSkipGeni2Checked;

    public bool IsSkipGeni2Checked
    {
        get => _isSkipGeni2Checked;
        set
        {
            if (SetProperty(ref _isSkipGeni2Checked, value)) Set(nameof(IsSkipGeni2Checked), value);
        }
    }

    private bool _isSkipEmmaChecked;

    public bool IsSkipEmmaChecked
    {
        get => _isSkipEmmaChecked;
        set
        {
            if (SetProperty(ref _isSkipEmmaChecked, value)) Set(nameof(IsSkipEmmaChecked), value);
        }
    }

    // New Game Cycle
    private bool _isAutoSetNewGame7Checked;

    public bool IsAutoSetNewGame7Checked
    {
        get => _isAutoSetNewGame7Checked;
        set
        {
            if (SetProperty(ref _isAutoSetNewGame7Checked, value)) Set(nameof(IsAutoSetNewGame7Checked), value);
        }
    }

    private bool _isDemonBellOnChecked;

    public bool IsDemonBellOnChecked
    {
        get => _isDemonBellOnChecked;
        set
        {
            if (SetProperty(ref _isDemonBellOnChecked, value)) Set(nameof(IsDemonBellOnChecked), value);
        }
    }

    private bool _isDemonBellOffChecked;

    public bool IsDemonBellOffChecked
    {
        get => _isDemonBellOffChecked;
        set
        {
            if (SetProperty(ref _isDemonBellOffChecked, value)) Set(nameof(IsDemonBellOffChecked), value);
        }
    }

    private bool _isNoKurosCharmOnChecked;

    public bool IsNoKurosCharmOnChecked
    {
        get => _isNoKurosCharmOnChecked;
        set
        {
            if (SetProperty(ref _isNoKurosCharmOnChecked, value)) Set(nameof(IsNoKurosCharmOnChecked), value);
        }
    }

    private bool _isNoKurosCharmOffChecked;

    public bool IsNoKurosCharmOffChecked
    {
        get => _isNoKurosCharmOffChecked;
        set
        {
            if (SetProperty(ref _isNoKurosCharmOffChecked, value)) Set(nameof(IsNoKurosCharmOffChecked), value);
        }
    }

    #endregion

    private void RegisterActions()
    {
        _isInfiniteConfettiChecked = Get(nameof(IsInfiniteConfettiChecked));
        _isInfiniteGachiinChecked = Get(nameof(IsInfiniteGachiinChecked));
        _isOneShotHealthChecked = Get(nameof(IsOneShotHealthChecked));
        _isOneShotPostureChecked = Get(nameof(IsOneShotPostureChecked));
        _isNoGoodsConsumeChecked = Get(nameof(IsNoGoodsConsumeChecked));
        _isNoEmblemConsumeChecked = Get(nameof(IsNoEmblemConsumeChecked));
        _isInfiniteRevivalChecked = Get(nameof(IsInfiniteRevivalChecked));
        _isPlayerHideChecked = Get(nameof(IsPlayerHideChecked));
        _isPlayerSilentChecked = Get(nameof(IsPlayerSilentChecked));
        _isInfinitePoiseChecked = Get(nameof(IsInfinitePoiseChecked));
        _isNoDeathChecked = Get(nameof(IsNoDeathChecked));
        _isNoDeathExKillboxChecked = Get(nameof(IsNoDeathExKillboxChecked));
        _isNoDamageChecked = Get(nameof(IsNoDamageChecked));
        _isToggleDamageMultiplierChecked = Get(nameof(IsToggleDamageMultiplierChecked));
        _damageMultiplierValue = _aol.GetDouble(nameof(DamageMultiplierValue), defaultValue: 1.0);

        _isSkipDragonPhaseOneChecked = Get(nameof(IsSkipDragonPhaseOneChecked));
        _isTriggerDragonFinalAttackChecked = Get(nameof(IsTriggerDragonFinalAttackChecked));
        _isNoButterflySummonsChecked = Get(nameof(IsNoButterflySummonsChecked));
        _isSnakeIntroLoopChecked = Get(nameof(IsSnakeIntroLoopChecked));

        _isEnableTargetOptionsChecked = Get(nameof(IsEnableTargetOptionsChecked));

        _isSkipGeni3Checked = Get(nameof(IsSkipGeni3Checked));
        _isSkipGeni2Checked = Get(nameof(IsSkipGeni2Checked));
        _isSkipEmmaChecked = Get(nameof(IsSkipEmmaChecked));

        _isAutoSetNewGame7Checked = Get(nameof(IsAutoSetNewGame7Checked));
        _isDemonBellOnChecked = Get(nameof(IsDemonBellOnChecked));
        _isDemonBellOffChecked = Get(nameof(IsDemonBellOffChecked));
        _isNoKurosCharmOnChecked = Get(nameof(IsNoKurosCharmOnChecked));
        _isNoKurosCharmOffChecked = Get(nameof(IsNoKurosCharmOffChecked));
    }

    private void OnAppStart()
    {
        if (!IsEnabled) return;

        // Player
        if (IsInfiniteConfettiChecked) _playerViewModel.IsConfettiFlagEnabled = true;
        if (IsInfiniteGachiinChecked) _playerViewModel.IsGachiinFlagEnabled = true;
        if (IsOneShotHealthChecked) _playerViewModel.IsOneShotEnabled = true;
        if (IsOneShotPostureChecked) _playerViewModel.IsOneShotPostureEnabled = true;
        if (IsNoGoodsConsumeChecked) _playerViewModel.IsNoGoodsConsumeEnabled = true;
        if (IsNoEmblemConsumeChecked) _playerViewModel.IsNoEmblemConsumeEnabled = true;
        if (IsInfiniteRevivalChecked) _playerViewModel.IsNoRevivalConsumeEnabled = true;
        if (IsPlayerHideChecked) _playerViewModel.IsPlayerHideEnabled = true;
        if (IsPlayerSilentChecked) _playerViewModel.IsPlayerSilentEnabled = true;
        if (IsInfinitePoiseChecked) _playerViewModel.IsInfinitePoiseEnabled = true;
        if (IsNoDeathChecked) _playerViewModel.IsNoDeathEnabled = true;
        if (IsNoDeathExKillboxChecked) _playerViewModel.IsNoDeathEnabledWithoutKillbox = true;
        if (IsNoDamageChecked) _playerViewModel.IsNoDamageEnabled = true;
        if (IsToggleDamageMultiplierChecked)
        {
            _playerViewModel.DamageMultiplier = DamageMultiplierValue;
            _playerViewModel.IsDamageMultiplierEnabled = true;
        }

        // Enemies
        if (IsNoButterflySummonsChecked) _enemyViewModel.IsNoButterflySummonsEnabled = true;
        if (IsSnakeIntroLoopChecked) _enemyViewModel.IsSnakeIntroLoopEnabled = true;

        // Target
        if (IsEnableTargetOptionsChecked) _targetViewModel.IsTargetOptionsEnabled = true;
    }

    private void OnGameAttached()
    {
        // No attach-gated Activate On Launch options for Sekiro yet (e.g. TarnishedTool's launch FPS);
        // kept for lifecycle parity with the other tools.
        if (!IsEnabled) return;
    }

    private void OnGameLoaded()
    {
        if (!IsEnabled) return;

        // Enemies
        if (IsSkipDragonPhaseOneChecked) _enemyViewModel.SkipDragonPhaseOneCommand.Execute(null);
        if (IsTriggerDragonFinalAttackChecked) _enemyViewModel.TriggerDragonFinalAttackCommand.Execute(null);

        // Boss Skips
        if (IsSkipGeni3Checked) _enemyViewModel.SkipGeni3Command.Execute(null);
        if (IsSkipGeni2Checked) _enemyViewModel.SkipTowerGeniCommand.Execute(null);
        if (IsSkipEmmaChecked) _enemyViewModel.SetEmmaSkipCommand.Execute(null);
    }

    private void OnNewGameStart()
    {
        if (!IsEnabled) return;

        if (IsAutoSetNewGame7Checked) _playerViewModel.IsAutoSetNewGameSevenEnabled = true;

        if (IsDemonBellOnChecked) _eventViewModel.SetDemonBellCommand.Execute(true);
        if (IsDemonBellOffChecked) _eventViewModel.SetDemonBellCommand.Execute(false);
        if (IsNoKurosCharmOnChecked) _eventViewModel.SetNoKurosCharmCommand.Execute(true);
        if (IsNoKurosCharmOffChecked) _eventViewModel.SetNoKurosCharmCommand.Execute(false);
    }
}
