using Godot;
using System;
using System.Collections.Generic;

public partial class NetworkClient : Node
{
    public static NetworkClient Instance { get; private set; }
    
    public string LocalUserID { get; private set; }
    public string CurrectRoomId { get; private set; } = "room1";
    public bool OfflineMode { get; private set; }
    
    private WebSocketPeer _webSocket = new WebSocketPeer();
    private PlayerManager _playerManager;   
    public event Action<string> Connected;
    
    private bool _wasOpen;
    public bool IsOpen => _webSocket?.GetReadyState() == WebSocketPeer.State.Open;
    public void SetLocalUserId(string localUserId) => LocalUserID = localUserId;
    
    
    public override void _Ready()
    {
        GD.Print("NetworkClient готов.");
        Instance = this;
    }
    
    public void EnableOfflineMode()
    {
        OfflineMode = true;
        LocalUserID = "local_player";
    }
    
    public override void _Process(double delta)
    {
        _webSocket.Poll();

        switch (_webSocket.GetReadyState())
        {
            case WebSocketPeer.State.Open:
                if (!_wasOpen)
                {
                    _wasOpen = true;
                    Connected?.Invoke("andrew" + Random.Shared.Next());  
                }
                break;
            case WebSocketPeer.State.Closed:
                if (_wasOpen)
                {
                    GD.PrintErr("WebSocket connection closed or errored.");
                    _wasOpen = false;
                }
                break;
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
        
        if (OfflineMode)
        {
            _playerManager?.SpawnLocalPlayer(LocalUserID, new Vector2(0, 0));
        }
    }

    public bool ConnectToServer(string url)
    {
        if (OfflineMode)
        {
            GD.Print("Игра запущена в оффлайн-режиме.");
            return true;
        }
        
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
        if (OfflineMode || !IsOpen)
            return;
        
        _webSocket.SendText(Json.Stringify(payload));
    }
    
    private Godot.Collections.Dictionary<string, Variant> BuildPlayerDto(string id, string nickname, Vector2 pos, string state)
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            { "id", id },
            { "nickname", nickname },
            { "x", pos.X},
            { "y", pos.Y },
            { "currentState", state }
        };
    }

    public void SendJoinRequest(string displayName)
    {
        var playerDto = BuildPlayerDto(LocalUserID, displayName, Vector2.Zero, "Idle");

        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "JOIN" },
            { "roomId", CurrectRoomId },
            { "player", playerDto }
        });
    }
    
    private Godot.Collections.Dictionary<string, Variant> PlayerMoveDtoToDict(PlayerMoveDto dto)
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            { "id", dto.Id },
            { "x", dto.X },
            { "y", dto.Y },
            { "dirX", dto.DirX },
            { "dirY", dto.DirY },
            { "currentState", dto.State ?? "Idle" }
        };
    }

    public void SendMoveRequest(PlayerMoveDto playerMoveDto)
    {
        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "MOVE" },
            { "roomId", CurrectRoomId },
            { "player",  PlayerMoveDtoToDict(playerMoveDto) }
        });
    }

    public void SendStateRequest(string playerId, string currentState)
    {
        var playerDto = new Godot.Collections.Dictionary<string, Variant>
        {
            { "id", playerId },
            { "currentState", currentState }
        };

        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "STATE_CHANGED" },
            { "roomId", CurrectRoomId },
            { "player", playerDto }
        });
    }
    
    public void SendLeaveRequest(string playerId) 
    {
        var playerDto = new Godot.Collections.Dictionary<string, Variant>
        {
            { "id", playerId }
        };

        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "LEAVE" },
            { "roomId", CurrectRoomId },
            { "player", playerDto }
        });
    }

    public void HandleServerMessage(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Nil)
            return;

        var message = parsed.AsGodotDictionary();
        var type = message.GetValueOrDefault("type", "").AsStringName().ToString();
    
        var playerDict = message.GetValueOrDefault("player", "").AsGodotDictionary();
        if (playerDict == null)
            return;

        var playerId = playerDict.GetValueOrDefault("id", "").AsString();
        var position = new Vector2(
            (float)playerDict.GetValueOrDefault("x", 0).AsDouble(),
            (float)playerDict.GetValueOrDefault("y", 0).AsDouble()
        );
        var direction = new Vector2(
            (float)playerDict.GetValueOrDefault("dirX", 0).AsDouble(),
            (float)playerDict.GetValueOrDefault("dirY", 0).AsDouble()
        );
        var velocity = new Vector2(
            (float)playerDict.GetValueOrDefault("velX", 0).AsDouble(),
            (float)playerDict.GetValueOrDefault("velY", 0).AsDouble()
        );
        var state = playerDict.GetValueOrDefault("currentState", "Idle").AsStringName().ToString();

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
                _playerManager?.OnPlayerMoved(playerId, position, direction, velocity);
                break;
            case "LEAVE":
                GD.Print("Пакет LEAVE:", json);
                _playerManager?.OnPlayerLeave(playerId);
                break;
            case "STATE_CHANGED":
                GD.Print("Пакет STATE:", json);
                _playerManager?.OnPlayerStateChanged(playerId, state);
                break;
        }
    }
    
    private void HandleJoinAck(Godot.Collections.Dictionary msg)
    {
        if (msg == null)
            return;

        LocalUserID = msg["playerId"].AsString();

        Vector2 pos = Vector2.Zero;

        var playerDict = msg.GetValueOrDefault("player", "").AsGodotDictionary();
        if (playerDict != null)
        {
            pos = new Vector2((float)playerDict.GetValueOrDefault("x", 0).AsDouble(),
                (float)playerDict.GetValueOrDefault("y", 0).AsDouble());
        }

        _playerManager.SpawnLocalPlayer(LocalUserID, pos);

        if (!msg.ContainsKey("existingPlayers")) 
            return;

        foreach (Variant entry in (Godot.Collections.Array)msg["existingPlayers"])
        {
            var dict = entry.AsGodotDictionary();
            var otherId = dict.GetValueOrDefault("id", "").AsString();
            var otherPos = new Vector2(
                (float)dict.GetValueOrDefault("x", 0).AsDouble(),
                (float)dict.GetValueOrDefault("y", 0).AsDouble()
            );
            _playerManager.OnPlayerJoined(otherId, otherPos);
        }
    }

}
