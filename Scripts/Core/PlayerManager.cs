using System;
using System.Collections.Generic;
using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Characters.States;

public partial class PlayerManager : Node
{
    [Export] private PackedScene _playerScene;
    [Export] private Marker2D _spawnPoint;
    private Godot.Collections.Dictionary<string, Player> _players = new();    
    
    public Player SpawnLocalPlayer(string playerId, Vector2 position)
    {
        if(_players.TryGetValue(playerId, out var existingPlayer))
            return existingPlayer;
        
        var player = CreatePlayerInstance(
            playerId,
            position,
            ControlMode.Local,
            NetworkClient.Instance.UserName
        );
        // var player = CreatePlayerInstance(playerId, _spawnPoint.Position, ControlMode.Local, nickname);
        player.SetNetworkId(playerId);
        player.InitializeStateMap();
        _players.Add(playerId, player);
        
        return player;
    }
    
    private Player CreatePlayerInstance(string playerId, Vector2 position, ControlMode mode, String nickname)
    {
        var localPlayer = _playerScene.Instantiate<Player>();
        localPlayer.Name = $"Player_{playerId}";
        localPlayer.Position = position;
        localPlayer.SetDisplayName(nickname);
        localPlayer.SetControlMode(mode);
        AddChild(localPlayer);
        
        return localPlayer;
    }
    
    public void OnPlayerJoined(string playerId, Vector2 position, string nickname)
    {
        if (_players.ContainsKey(playerId))
            return;
        
        var joinedPlayer = _playerScene.Instantiate<Player>();
        joinedPlayer.Name = $"Player_{playerId}";
        joinedPlayer.Position = position;
        joinedPlayer.SetDisplayName(nickname);
        joinedPlayer.SetNetworkId(playerId);
        joinedPlayer.SetControlMode(ControlMode.Remote);
        AddChild(joinedPlayer); 
        joinedPlayer.InitializeStateMap();
        _players[playerId] = joinedPlayer;
        
    }

    public void OnPlayerMoved(string playerId, Vector2 position, Vector2 dir, Vector2 velocity)
    {
        if (!_players.TryGetValue(playerId, out var player)) 
            return;

        player.SetRemoteInput(dir, position, velocity);
    }

    public void OnPlayerStateChanged(string playerId, string state)
    {
        if (!_players.TryGetValue(playerId, out var player)) 
            return;

        if (Enum.TryParse<PlayerStateType>(state, out var parsedState))
        {
            var newState = player.MapEnumToState(parsedState);
            if (newState != null)
                player.StateMachine.ChangeState(newState);
        }
    }

    public void OnPlayerLeave(string playerId)
    {
        if (!_players.TryGetValue(playerId, out var player))
            return;
        
        player.QueueFree();
        _players.Remove(playerId);
    }
    
    public Godot.Collections.Dictionary<string, Player> GetPlayers() => _players;
    
    public Player GetPlayer(string playerId)
    {
        return _players.GetValueOrDefault(playerId);
    }
}
