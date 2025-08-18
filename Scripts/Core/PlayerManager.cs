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
        var localPlayer = _playerScene.Instantiate<Player>();
        localPlayer.Name = $"Player_{playerId}";
        localPlayer.Position = position;
        localPlayer.IsLocal = isLocal;
        localPlayer.SetDisplayName(playerId);
        AddChild(localPlayer);
        
        return localPlayer;
    }
    
    public void OnPlayerJoined(string playerId, Vector2 position)
    {
        if (_players.ContainsKey(playerId))
            return;
        
        var joinedPlayer = _playerScene.Instantiate<Player>();
        joinedPlayer.Name = $"Player_{playerId}";
        joinedPlayer.Position = position;
        joinedPlayer.IsLocal = false;
        joinedPlayer.SetDisplayName(playerId);
        AddChild(joinedPlayer);
        _players[playerId] = joinedPlayer;
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
