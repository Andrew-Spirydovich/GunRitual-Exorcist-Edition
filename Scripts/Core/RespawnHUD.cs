using Godot;
namespace GunRitualExorcistEdition.Scripts.Core;

public partial class RespawnHUD : Control
{
    [Export] private Button _respawnButton;
    [Export] private Button _leaveButton;

    public override void _Ready()
    {
        Visible = false;

        _respawnButton.Pressed += OnRespawnPressed;
    }

    private void OnRespawnPressed()
    {
        NetworkClient.Instance.SendRespawnRequest();
        Hide();
    }

    public void ShowUI() => Visible = true;
    public void HideUI() => Visible = false;
}