using H.Hooks;
using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.Utilities;

public class HotkeyManager
{
    private readonly IMemoryService _memoryService;
    private readonly LowLevelKeyboardHook _keyboardHook = new();
    private readonly Dictionary<string, Keys> _hotkeyMappings = new();
    private readonly Dictionary<string, Action> _actions = new();
    private readonly Dictionary<string, Action> _startupActions = new();
    private readonly Dictionary<string, Action> _newGameActions = new();

    public HotkeyManager(IMemoryService memoryService)
    {
        _memoryService = memoryService;
        
        _keyboardHook.HandleModifierKeys = true;
        _keyboardHook.Down += KeyboardHook_Down;
        LoadHotkeys();
        if (SettingsManager.Default.EnableHotkeys) _keyboardHook.Start();
    }
    
    public void Start()
    {
        _keyboardHook.Start();
    }
        
    public void Stop()
    {
        _keyboardHook.Stop();
    }
    
    public void RegisterAction(HotkeyActions actionId, Action action)
    {
        _actions[actionId.ToString()] = action;
    }

    public void TriggerAction(string actionId)
    {
        if (_actions.TryGetValue(actionId, out var action))
            action.Invoke();
    }

    // Some actions registered via RegisterAction are toggles (flip current state),
    // which is correct for a manually-pressed hotkey but unsafe to call more than
    // once (e.g. applying startup options on both game load and a later New Game).
    // Register an idempotent "ensure enabled" variant here for those; actions with
    // no dedicated startup variant fall back to the regular toggle action.
    public void RegisterStartupAction(HotkeyActions actionId, Action action)
    {
        _startupActions[actionId.ToString()] = action;
    }

    public void TriggerStartupAction(string actionId)
    {
        if (_startupActions.TryGetValue(actionId, out var startupAction))
            startupAction.Invoke();
        else
            TriggerAction(actionId);
    }

    // A small number of actions need different handling specifically when a New
    // Game is detected (e.g. Max HP needs to wait out the engine's own starting-HP
    // reset instead of applying immediately). Actions without a dedicated New Game
    // variant fall back to the regular startup action.
    public void RegisterNewGameAction(HotkeyActions actionId, Action action)
    {
        _newGameActions[actionId.ToString()] = action;
    }

    public void TriggerNewGameAction(string actionId)
    {
        if (_newGameActions.TryGetValue(actionId, out var newGameAction))
            newGameAction.Invoke();
        else
            TriggerStartupAction(actionId);
    }


    private void KeyboardHook_Down(object sender, KeyboardEventArgs e)
    {
        if (!IsGameFocused())
            return;
        foreach (var mapping in _hotkeyMappings)
        {
            string actionId = mapping.Key;
            Keys keys = mapping.Value;
            if (!e.Keys.Are(keys.Values.ToArray())) continue;
            if (_actions.TryGetValue(actionId, out var action))
            {
                action.Invoke();
            }
            break;
        }
    }
    
    private bool IsGameFocused()
    {
        if (_memoryService.TargetProcess == null || _memoryService.TargetProcess.Id == 0) return false;
         
        IntPtr foregroundWindow = User32.GetForegroundWindow();
        User32.GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
        return foregroundProcessId == (uint)_memoryService.TargetProcess.Id;
    }
    
    public void SetHotkey(string actionId, Keys keys)
    {
        _hotkeyMappings[actionId] = keys;
        SaveHotkeys();
    }
        
    public void ClearHotkey(string actionId)
    {
        _hotkeyMappings.Remove(actionId);
        SaveHotkeys();
    }
    

   public Keys GetHotkey(string actionId)
        {
            return _hotkeyMappings.TryGetValue(actionId, out var keys) ? keys : null;
        }
        
        public string GetActionIdByKeys(Keys keys)
        {
            return _hotkeyMappings.FirstOrDefault(x => x.Value == keys).Key;
        }
        
        
        public void SaveHotkeys()
        {
            try
            {
                var mappingPairs = new List<string>();
        
                foreach (var mapping in _hotkeyMappings)
                {
                    mappingPairs.Add($"{mapping.Key}={mapping.Value}");
                }
                
                SettingsManager.Default.HotkeyActionIds = string.Join(";", mappingPairs);
        
                SettingsManager.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving hotkeys: {ex.Message}");
            }
        }

        public void LoadHotkeys()
        {
            try
            {
                _hotkeyMappings.Clear();
        
                string mappingsString = SettingsManager.Default.HotkeyActionIds;
        
                if (!string.IsNullOrEmpty(mappingsString))
                {
                    string[] pairs = mappingsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
                    foreach (string pair in pairs)
                    {
                        int separatorIndex = pair.IndexOf('=');
                        if (separatorIndex > 0)
                        {
                            string actionId = pair.Substring(0, separatorIndex);
                            string keyValue = pair.Substring(separatorIndex + 1);
                    
                            _hotkeyMappings[actionId] = Keys.Parse(keyValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading hotkeys: {ex.Message}");
            }
        }

        public void ClearAll()
        {
            _hotkeyMappings.Clear();
            SaveHotkeys();
        }
}