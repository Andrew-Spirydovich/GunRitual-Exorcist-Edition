using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;


public partial class NetworkClient : Node
{
    [Signal]
    public delegate void ConnectedEventHandler();
    public static NetworkClient Instance { get; private set; }
    
    private string _localPlayerId;
    
    private WebSocketPeer _webSocket = new WebSocketPeer();
    private PlayerManager _playerManager;   
    
    public bool IsOpen => _webSocket?.GetReadyState() == WebSocketPeer.State.Open; // вот это надо убрать
    public void SetLocalPlayerId(string localPlayerId) => _localPlayerId = localPlayerId;
    public string GetLocalPlayerId() => _localPlayerId;
    
    public override void _Ready()
    {
        GD.Print("NetworkClient готов.");
        Instance = this;
    }

    public override void _Process(double delta)
    {
        _webSocket.Poll();
        
        if (IsOpen)
        {
            EmitSignal(SignalName.Connected); // вот это надо убрать
            while (_webSocket.GetAvailablePacketCount() > 0)
            {
                var msg = _webSocket.GetPacket().GetStringFromUtf8();
                HandleServerMessage(msg);
            }
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

    public void SendJoin(string playerId, Vector2 position)
    {
        if (!IsOpen)
        {
            GD.Print("Send skipped: SenJoin");
            return;
        }

        var payload = new Godot.Collections.Dictionary<string, Variant>
        {
            {"type", "JOIN"},
            {"playerId", playerId},
            {"x", position.X.ToString(CultureInfo.InvariantCulture)},
            {"y", position.Y.ToString(CultureInfo.InvariantCulture)}
        };
        
        var json = Json.Stringify(payload);
        GD.Print("SEND:", json);
        _webSocket.SendText(json);
    }

    public void SendMove(string playerId, Vector2 position)
    {
        var payload = new Godot.Collections.Dictionary<string, Variant>
        {
            {"type", "MOVE"},
            {"playerId", playerId},
            {"x", position.X},
            {"y", position.Y}
        };
        
        _webSocket.SendText(Json.Stringify(payload));
    }

    public void SendLeave(string playerId)
    {
        var payload = new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "LEAVE" },
            { "playerId", playerId }
        };
        
        _webSocket.SendText(Json.Stringify(payload));
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

        switch (type)
        {
            case "JOIN":
                _playerManager?.OnPlayerJoined(playerId, pos);
                break;
            case "MOVE":
                _playerManager?.OnPlayerMoved(playerId, pos);
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
