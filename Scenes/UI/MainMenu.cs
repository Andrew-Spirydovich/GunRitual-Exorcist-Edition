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
    [Export]
    private NetworkClient _client;

    public override void _Ready()
    {
        _playButton.Pressed += OnPlayPressed;
        _settingsButton.Pressed += OnSettingsPressed;
        _hostButton.Pressed += OnHostPressed;
        _quitButton.Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://GameScene.tscn");
    }

    private void OnHostPressed()
    {
        _client.SendMessage("Мы создали хост");
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
