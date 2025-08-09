using Godot;
using Godot.Collections;

public partial class PlayerManager : Node
{
    [Export] 
    private PackedScene _playerScene;
    private Dictionary<string, Player> _players = new Dictionary<string,Player>();
    
    public Player SpawnLocalPlayer(string playerId, Vector2 position)
    {
        if(_players.TryGetValue(playerId, out var existingPlayer))
            return existingPlayer;

        var localPlayer = _playerScene.Instantiate<Player>();
        localPlayer.Name = $"Player_{playerId}";
        localPlayer.Position = Vector2.Zero;
        localPlayer.IsLocal = true;
        AddChild(localPlayer);
        _players.Add(playerId, localPlayer);
        
        NetworkClient.Instance?.SendJoin(playerId, position);
        return localPlayer;
    }
    
    public void OnPlayerJoined(string playerId, Vector2 position)
    {
        if (_players.ContainsKey(playerId))
            return;
        
        var playerInstance = _playerScene.Instantiate<Player>();
        playerInstance.Name = $"Player_{playerId}";
        playerInstance.Position = position;
        playerInstance.IsLocal = false;
        AddChild(playerInstance);
        _players[playerId] = playerInstance;
    }

    public void OnPlayerMoved(string playerId, Vector2 position)
    {
        if (_players.TryGetValue(playerId, out var player))
            player.Position = position;
    }

    public void OnPlayerLeave(string playerId)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.QueueFree();
            _players.Remove(playerId);
        }
    }
}
