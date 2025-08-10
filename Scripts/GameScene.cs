using Godot;
using System;

public partial class GameScene : Node2D
{
    [Export] private PlayerManager _playerManager;

    public override void _Ready()
    {
        var net = NetworkClient.Instance;
        net.SetPlayerManager(_playerManager);
        net.Connected += OnNetConnected;
    }

    private void OnNetConnected()
    {
        _playerManager.SpawnLocalPlayer(NetworkClient.Instance?.GetLocalUserId(), new Vector2(0, 0));
    }

}
