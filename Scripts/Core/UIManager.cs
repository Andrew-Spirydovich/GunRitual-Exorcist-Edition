using System.Collections.Generic;
using Godot;


namespace GunRitualExorcistEdition.Scripts.Core;

public partial class UIManager : Node
{
    public static UIManager Instance { get; private set; }

    [Export] private RespawnHUD _respawnHud;
    [Export] private ExitMenu _exitMenu;
    [Export] private RecordHUD _recordHud;
    [Export] private Label _timeLabel;

    public override void _Ready()
    {
        Instance = this;

        _respawnHud.HideUI();
        _exitMenu.HideUI();
        _recordHud.HideUI();
    }

    public override void _Process(double delta)
    {
        var context = InputManager.GetContext<ControlContext>();

        if (context.IsEscapePressed)
            ShowExitMenu();

        if (context.IsRecordPressed)
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
    
    public void SetTime(int seconds)
    {
        var min = seconds / 60;
        var sec = seconds % 60;
        _timeLabel.Text = $"{min:00}:{sec:00}";
    }

    public void ShowGameOver()
    {
        _timeLabel.Text = "GAME OVER";
    }

    public void UpdateScores(Dictionary<string, int> scores)
    {
        _recordHud.UpdateScores(scores);
    }
    
    private void HandleGameFinished(Godot.Collections.Dictionary msg)
    {
        UIManager.Instance.ShowGameOver();

        if (msg.ContainsKey("scores"))
        {
            var scoresDict = msg["scores"].AsGodotDictionary();
            var scores = new Dictionary<string, int>();

            foreach (var key in scoresDict.Keys)
                scores[key.ToString()] = (int)scoresDict[key].AsInt32();

            // Показываем RecordHUD с финальными очками
            UIManager.Instance.ShowRecordHud();
            _recordHud.UpdateScores(scores);
        }
    }
}   