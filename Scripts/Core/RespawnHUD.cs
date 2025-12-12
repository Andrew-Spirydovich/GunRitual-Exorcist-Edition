using Godot;
namespace GunRitualExorcistEdition.Scripts.Core;

public partial class RespawnHUD : Control
{
    [Export] private Button _respawnButton;
    [Export] private Button _leaveButton;
    
    public override void _Ready()
    {
        Visible = false;
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