using Godot;
using System;
using System.Collections.Generic;

public partial class NetworkClient : Node
{
    public static NetworkClient Instance { get; private set; }
    
    public string LocalUserID { get; private set; }
    private WebSocketPeer _webSocket = new WebSocketPeer();
    private PlayerManager _playerManager;   
    
    public event Action<string> Connected;
    
    private bool _wasOpen;
    public bool IsOpen => _webSocket?.GetReadyState() == WebSocketPeer.State.Open;
    
    public override void _Ready()
    {
        GD.Print("NetworkClient готов.");
        Instance = this;
    }
    
    public void SetLocalUserId(string localUserId) => LocalUserID = localUserId;

    public override void _Process(double delta)
    {
        _webSocket.Poll();

        if (IsOpen && !_wasOpen)
        {
            _wasOpen = true;
            Connected?.Invoke("andrew" + Random.Shared.Next());
        }
        
        while (_webSocket.GetAvailablePacketCount() > 0)
        {
            var msg = _webSocket.GetPacket().GetStringFromUtf8();
            HandleServerMessage(msg);
        }
        
    }
    
    public void SetPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    public bool ConnectToServer(string url)
    {
        var err = _webSocket.ConnectToUrl(url);
        if (err != Error.Ok)
        {
            GD.PrintErr("Ошибка подключения к серверу: " + err);
            return false;
        }
        
        return true;
    }

    private void SendMessage(Godot.Collections.Dictionary<string, Variant> payload)
    {
        if (!IsOpen)
        {
            GD.Print($"Send skipped: {payload["type"]}");
            return;
        }
        _webSocket.SendText(Json.Stringify(payload));
    }
    
    public void SendJoinRequest(string displayName) 
        => SendMessage(new Godot.Collections.Dictionary<string, Variant>
    {
        { "type", "JOIN" },
        { "roomId", "room1" },
        { "displayName", displayName }
    });
    

    public void SendMoveRequest(string playerId, Vector2 position, Vector2 direction) 
        => SendMessage(new Godot.Collections.Dictionary<string, Variant>
    {
            { "type", "MOVE" },
            { "playerId", playerId },
            { "roomId", "room1" },
            { "x", position.X },
            { "y", position.Y },
            { "dirX", direction.X },
            { "dirY", direction.Y }
    });
    
    public void SendLeaveRequest(string playerId) 
        => SendMessage(new Godot.Collections.Dictionary<string, Variant>
    {
        { "type", "LEAVE" },
        { "roomId", "room1" },
        { "playerId", playerId }
    });

    public void HandleServerMessage(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Nil)
            return;

        var message = parsed.AsGodotDictionary();
        var type = message.GetValueOrDefault("type", "").AsStringName().ToString();
        var playerId = message.GetValueOrDefault("playerId", "").AsStringName().ToString();
        
        var position = new Vector2((float)message.GetValueOrDefault("x", 0).AsDouble(),
            (float)message.GetValueOrDefault("y", 0).AsDouble());
        
        var direction = new Vector2((float)message.GetValueOrDefault("dirX", 0).AsDouble(),
            (float)message.GetValueOrDefault("dirY", 0).AsDouble());
        
        switch (type)
        {
            case "JOIN_ACK":
                HandleJoinAck(message);
                break;
            case "JOIN":
                GD.Print("Пакет JOIN:", json);
                _playerManager?.OnPlayerJoined(playerId, position);
                break;
            case "MOVE":
                _playerManager?.OnPlayerMoved(playerId, position, direction);
                break;
            case "LEAVE":
                GD.Print("Пакет LEAVE:", json);
                _playerManager?.OnPlayerLeave(playerId);
                break;
        }
    }
    
    private void HandleJoinAck(Godot.Collections.Dictionary msg)
    {
        LocalUserID = msg["playerId"].AsString();
        var pos = new Vector2((float)msg["x"].AsDouble(), (float)msg["y"].AsDouble());
        _playerManager.SpawnLocalPlayer(LocalUserID, pos);

        if (!msg.ContainsKey("existingPlayers")) 
            return;
        
        foreach (Variant entry in (Godot.Collections.Array)msg["existingPlayers"])
        {
            var dict = entry.AsGodotDictionary();
            var otherId = dict["playerId"].AsString();
            var otherPos = new Vector2((float)dict["x"].AsDouble(), (float)dict["y"].AsDouble());
            _playerManager.OnPlayerJoined(otherId, otherPos);
        }
    }

}
