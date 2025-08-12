using Godot;
using System;

public partial class GameScene : Node2D
{
    [Export] private PlayerManager _playerManager;

    public override void _Ready()
    {
        var net = NetworkClient.Instance;
        net.SetPlayerManager(_playerManager);

        net.Connected += net.SendJoinRequest;
        
        if(net.IsOpen)
            net.SendJoinRequest("andrew");
    }
}
