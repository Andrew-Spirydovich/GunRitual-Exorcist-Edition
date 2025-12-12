using Godot;

public partial class GameManager : Node2D
{
    [Export] private PlayerManager _playerManager;
    [Export] private BulletManager _bulletManager;
    
    public override void _Ready()
    {
        var net = NetworkClient.Instance;
        net.InitManagers(_playerManager,  _bulletManager);

        net.Connected += net.SendJoinRequest;
        
        if(net.IsOpen)
            net.SendJoinRequest("andrew");
    }
}
