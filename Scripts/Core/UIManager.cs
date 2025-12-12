using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public partial class UIManager : Node
{
    [Export] private RespawnHUD _respawnHud;
    [Export] private ExitMenu _exitMenu;
    [Export] private RecordHUD _recordHud;
    [Export] private Label _timeLabel;
    public override void _Ready()
    {
        _respawnHud.HideUI();
        _exitMenu.HideUI();
        _recordHud.HideUI();
    }
    
    public override void _Process(double delta)
    {
        var context = InputManager.GetContext<ControlContext>();
        
        if(context.IsEscapePressed)
            ShowExitMenu();
        
        if(context.IsRecordPressed)
            ShowRecordHud();
    }

    private void HideAll()
    {
        _respawnHud.HideUI();
        _exitMenu.HideUI();
        _recordHud.HideUI();
    }

    public void ShowRespawn()
    {
        HideAll();
        _respawnHud.ShowUI();
    }

    public void ShowExitMenu()
    {
        if (_exitMenu.Visible)
        {
            _exitMenu.HideUI();
            return;
        }
        
        HideAll();
        _exitMenu.ShowUI();
    }

    public void ShowRecordHud()
    {
        if (_recordHud.Visible)
        {
            _recordHud.HideUI();
            return;
        }
        
        HideAll();
        _recordHud.ShowUI();
    }
}