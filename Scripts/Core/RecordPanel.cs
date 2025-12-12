using Godot;
namespace GunRitualExorcistEdition.Scripts.Core;

public partial class RecordPanel : Control
{
    [Export] private Label _nickName;
    [Export] private Label _score;
    
    public void UpdateNickName(string nickName)
    {
        _nickName.Text = nickName;
    }

    public void UpdateScore(int score)
    {
        _score.Text = score.ToString();
    }
}