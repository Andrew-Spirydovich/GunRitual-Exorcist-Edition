using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public partial class ExitMenu : Control
{
    [Export] private Button _continueButton;
    [Export] private Button _leaveButton;
    
    public override void _Ready()
    {
        Visible = false;
        _continueButton.Pressed += HideUI;
        _leaveButton.Pressed += () => LeaveToMenu();
    }

    private void LeaveToMenu()
    {
        NetworkClient.Instance.SendLeaveRequest(NetworkClient.Instance.LocalUserID);
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
    
    public void HideUI()
    {
        Visible = false;
    }
    
    public void ShowUI()
    {
        Visible = true;
    }
}