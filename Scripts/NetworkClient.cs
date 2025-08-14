using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using Range = Godot.Range;


public partial class NetworkClient : Node
{
    public static NetworkClient Instance { get; private set; }
    
    private string _localUserId;
    
    private WebSocketPeer _webSocket = new WebSocketPeer();
    private PlayerManager _playerManager;   
    public event Action<string> Connected;
    private bool _wasOpen;
    public bool IsOpen => _webSocket?.GetReadyState() == WebSocketPeer.State.Open; // вот это надо убрать
    
    public void SetLocalUserId(string localUserId) => _localUserId = localUserId;
    public string GetLocalUserId() => _localUserId;
    
    public override void _Ready()
    {
        GD.Print("NetworkClient готов.");
        Instance = this;
    }

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

    public void SendJoinRequest(string displayName)
    {
        if (!IsOpen)
        {
            GD.Print("Send skipped: SenJoin");
            return;
        }
        
        var payload = new Godot.Collections.Dictionary<string,Variant>
        {
            { "type", "JOIN" },
            { "roomId", "room1" },
            { "displayName", displayName }
        };
        
        _webSocket.SendText(Json.Stringify(payload));
    }

    public void SendMoveRequest(string playerId, Vector2 position, Vector2 direction)
    {
        var payload = new Godot.Collections.Dictionary<string, Variant>
        {
            {"type", "MOVE"},
            {"playerId", playerId},
            {"roomId", "room1"},
            {"x", position.X},
            {"y", position.Y},
            {"dirX", direction.X},
            {"dirY", direction.Y}
        };
        
        var json = Json.Stringify(payload);
        GD.Print("Попытка отправки MOVE:", json);
        _webSocket.SendText(json);
    }

    public void SendLeaveRequest(string playerId)
    {
        var payload = new Godot.Collections.Dictionary<string, Variant>
        {
            {"type", "LEAVE" },
            {"roomId", "room1"},
            { "playerId", playerId }
        };
        
        var json = Json.Stringify(payload);
        GD.Print("Попытка отправки LEAVE:", json);
        _webSocket.SendText(json);
    }

    public void HandleServerMessage(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Nil)
            return;

        var msg = parsed.AsGodotDictionary();
        var type = msg.GetValueOrDefault("type", "").AsStringName().ToString();
        var playerId = msg.GetValueOrDefault("playerId", "").AsStringName().ToString();
        
        var x = (float)msg.GetValueOrDefault("x", 0).AsDouble();
        var y = (float)msg.GetValueOrDefault("y", 0).AsDouble();
        var pos = new Vector2(x, y);
        var dirX = (float)msg.GetValueOrDefault("dirX", 0).AsDouble();
        var dirY = (float)msg.GetValueOrDefault("dirY", 0).AsDouble();
        var dir = new Vector2(dirX, dirY);
        
        switch (type)
        {
            case "JOIN_ACK":
                GD.Print("JOIN_ACK:", json);
                playerId = msg["playerId"].AsString();
                x = (float)msg["x"].AsDouble();
                y = (float)msg["y"].AsDouble();
                pos = new Vector2(x, y);

                _localUserId = playerId;
                _playerManager.SpawnLocalPlayer(playerId, pos);

                if (msg.ContainsKey("existingPlayers"))
                {
                    var existing = (Godot.Collections.Array)msg["existingPlayers"];
                    foreach (Godot.Variant entry in existing)
                    {
                        var dict = entry.AsGodotDictionary();
                        var otherId = dict["playerId"].AsString();
                        var ox = (float)dict["x"].AsDouble();
                        var oy = (float)dict["y"].AsDouble();
                        _playerManager.OnPlayerJoined(otherId, new Vector2(ox, oy));
                    }
                }
                break;
            case "JOIN":
                GD.Print("JOIN:", json);
                _playerManager?.OnPlayerJoined(playerId, pos);
                break;
            case "MOVE":
                _playerManager?.OnPlayerMoved(playerId, pos, dir);
                break;
            case "LEAVE":
                _playerManager?.OnPlayerLeave(playerId);
                break;
        }
    }
    
    public void SetPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }
}
