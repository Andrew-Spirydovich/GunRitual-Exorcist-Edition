using Godot;
using Godot.Collections;

public partial class PlayerManager : Node
{
    [Export] private PackedScene _playerScene;
    private Dictionary<string, Player> _players = new();    
    
    public Player SpawnLocalPlayer(string playerId, Vector2 position)
    {
        if(_players.TryGetValue(playerId, out var existingPlayer))
            return existingPlayer;

        var player = CreatePlayerInstance(playerId, position, isLocal: true);
        _players.Add(playerId, player);
        
        return player;
    }
    
    private Player CreatePlayerInstance(string playerId, Vector2 position, bool isLocal)
    {
        var player = _playerScene.Instantiate<Player>();
        player.Name = $"Player_{playerId}";
        player.Position = position;
        player.IsLocal = isLocal;
        AddChild(player);
        
        return player;
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

    public void OnPlayerMoved(string playerId, Vector2 position, Vector2 dir)
    {
        if (!_players.TryGetValue(playerId, out var player)) 
            return;

        player.SetRemoteInput(dir, position);
    }

    public void OnPlayerLeave(string playerId)
    {
        if (!_players.TryGetValue(playerId, out var player))
            return;
        
        player.QueueFree();
        _players.Remove(playerId);
    }
}
