using Godot;
using System;

public partial class MainMenu : Control
{
    [Export]
    private Button _playButton;
    [Export]
    private Button _hostButton;
    [Export]
    private Button _connectButton;
    [Export]
    private Button _settingsButton;
    [Export]
    private Button _quitButton;

    public override void _Ready()
    {
        _playButton.Pressed += OnPlayPressed;
        _settingsButton.Pressed += OnSettingsPressed;
        _hostButton.Pressed += OnHostPressed;
        _quitButton.Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        var connected = NetworkClient.Instance.ConnectToServer("ws://192.168.31.73:8080/ws");

        if (connected)
        {
            NetworkClient.Instance?.SetLocalUserId("Andrew");
            GetTree().ChangeSceneToFile("res://GameScene.tscn");
        }
    }

    private void OnHostPressed()
    {
    }

    private void OnSettingsPressed()
    {
        GD.Print("Settings");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
