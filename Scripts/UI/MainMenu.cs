using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;

public partial class MainMenu : Control
{
    [Export] private Button _connectButton;
    [Export] private Button _settingsButton;
    [Export] private Button _exitButton;

    public override void _Ready()
    {
        _connectButton.Pressed += OnConnectPressed;
        _settingsButton.Pressed += OnSettingsPressed;
        _exitButton.Pressed += OnExitPressed;

        var mainMenuMusic = GD.Load<AudioStream>("res://Assets/Audio/main_menu.mp3");
        AudioManager.Instance.PlayMusic(mainMenuMusic, 0);
    }

    private void OnConnectPressed()
    {
        var connected = NetworkClient.Instance.ConnectToServer("ws://192.168.31.73:8080/ws");

        if (connected)
        {
            GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
        }
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
