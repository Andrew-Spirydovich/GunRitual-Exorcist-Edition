using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public partial class SettingsHUD : Control
{
    [Export] private Button _saveButton;
    [Export] private Button _leaveButton;
    [Export] private LineEdit _lineEdit;
    public override void _Ready()
    {
        Visible = false;
        _saveButton.Pressed += Save;
        _leaveButton.Pressed += Leave;
    }

    private void ShowUI()
    {
        Visible = true;
        if (NetworkClient.Instance.UserName != null)
        {
            _lineEdit.Text = NetworkClient.Instance.UserName;
        }
    }
    
    private void Save()
    {
        NetworkClient.Instance.UserName = _lineEdit.Text;
        _lineEdit.Clear();
        Visible = false;
        
    }

    private void Leave()
    {
        _lineEdit.Clear();
        Visible = false;
    }
}