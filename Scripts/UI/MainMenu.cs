using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;

public partial class MainMenu : Control
{
    [Export] private Button _connectButton;
    [Export] private Button _localButton;
    [Export] private Button _settingsButton;
    [Export] private Button _exitButton;
    
    [Export] private AudioStream _mainMenuMusic;
    
    public override void _Ready()
    {
        _connectButton.Pressed += OnConnectPressed;
        _localButton.Pressed += OnLocalConnectPressed;
        _settingsButton.Pressed += OnSettingsPressed;
        _exitButton.Pressed += OnExitPressed;
        
        AudioManager.Instance.PlayMusic(_mainMenuMusic, 0);
    }

    private void OnConnectPressed()
    {
        var connected = NetworkClient.Instance.ConnectToServer("ws://192.168.31.73:8080/ws");

        if (connected)
        {
            GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
        }
        AudioManager.Instance.StopMusic();
    }

    private void OnLocalConnectPressed()
    {
        NetworkClient.Instance.EnableOfflineMode();
        GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
        AudioManager.Instance.StopMusic();
    }

    private void OnSettingsPressed()
    {
        GD.Print("Settings");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
