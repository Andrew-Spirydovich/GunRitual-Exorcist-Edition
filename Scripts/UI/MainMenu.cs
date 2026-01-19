using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;

public partial class MainMenu : Control
{
    [Export] private Button _settingsButton;
    [Export] private Button _exitButton;
    [Export] private SettingsHUD _settingsHUD;
    
    [Export] private AudioStream _mainMenuMusic;
    
    public override void _Ready()
    {
        _settingsButton.Pressed += OnSettingsPressed;
        _exitButton.Pressed += OnExitPressed;
        
        //AudioManager.Instance.PlayMusic(_mainMenuMusic, 0);
    }
    
    private void OnLocalConnectPressed()
    {
        NetworkClient.Instance.EnableOfflineMode();
        GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
        AudioManager.Instance.StopMusic();
    }

    private void OnSettingsPressed()
    {
        _settingsHUD.Show();
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
