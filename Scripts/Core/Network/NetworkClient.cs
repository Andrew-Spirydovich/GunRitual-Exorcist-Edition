using Godot;
using System;
using System.Collections.Generic;
using GunRitualExorcistEdition.Scripts.Core;

public partial class NetworkClient : Node
{
    public static NetworkClient Instance { get; private set; }
    
    public string LocalUserID { get; private set; }
    public string CurrectRoomId { get; private set; } = "room1";
    public string UserName { get; set; }
    public bool OfflineMode { get; private set; }
    
    private WebSocketPeer _webSocket = new WebSocketPeer();
    public PlayerManager PlayerManager { get; private set; }
    private BulletManager _bulletManager;
    public event Action<string> Connected;
    
    private bool _wasOpen;
    public bool IsOpen => _webSocket?.GetReadyState() == WebSocketPeer.State.Open;
    public void SetLocalUserId(string localUserId) => LocalUserID = localUserId;
    private int _serverTimeLeft;
    
    public override void _Ready()
    {
        // GD.Print("NetworkClient готов.");
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
                    //Connected?.Invoke("andrew" + Random.Shared.Next());  
                    SendJoinRequest(UserName);
                }
                break;
            case WebSocketPeer.State.Closed:
                if (_wasOpen)
                {
                    // GD.PrintErr("WebSocket connection closed or errored.");
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
    
    public void InitManagers(PlayerManager playerManager, BulletManager bulletManager)
    {
        PlayerManager = playerManager;
        _bulletManager = bulletManager;
        
        if (OfflineMode)
        {
            PlayerManager?.SpawnLocalPlayer(LocalUserID, new Vector2(0, 0));
        }
    }

    public bool ConnectToServer(string url)
    {
        if (OfflineMode)
        {
            // GD.Print("Игра запущена в оффлайн-режиме.");
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
            { "nickname", Instance.UserName },
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
            { "nickname", Instance.UserName },
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

    public void SendShootRequest(BullletMoveDto dto)
    {
        var bulletDict = new Godot.Collections.Dictionary<string, Variant>
        {
            { "x", dto.X },
            { "y", dto.Y },
            { "dirX", dto.DirX },
            { "dirY", dto.DirY },
            { "ownerId", LocalUserID }
        };
        
        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "SHOOT" },
            { "roomId", CurrectRoomId },
            { "bullet", bulletDict }
        });
    }
    
    public void SendRespawnRequest()
    {
        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "RESPAWN_REQUEST" },
            { "roomId", CurrectRoomId },
            { "playerId", LocalUserID }
        });
    }

    public void SendBulletHitRequest(string bulletId, string ownerId, string targetPlayerId)
    {
        var bulletDict = new Godot.Collections.Dictionary<string, Variant>
        {
            { "id", bulletId },
            { "ownerId", ownerId },
            { "targetId", targetPlayerId }
        };

        SendMessage(new Godot.Collections.Dictionary<string, Variant>
        {
            { "type", "BULLET_HIT" },
            { "roomId", CurrectRoomId },
            { "bullet", bulletDict }
        });
    }

    private void HandleGameTime(Godot.Collections.Dictionary msg)
    {
        if (!msg.ContainsKey("time"))
            return;

        _serverTimeLeft = (int)msg["time"];
        UIManager.Instance.SetTime(_serverTimeLeft);
    }

    private void HandleGameFinished()
    {
        UIManager.Instance.ShowGameOver();
    }
    
    public void HandleScoreUpdate(Godot.Collections.Dictionary msg)
    {
        if (!msg.ContainsKey("scores"))
            return;

        var scoresDict = msg["scores"].AsGodotDictionary();
        var scores = new Dictionary<string, int>();

        foreach (var key in scoresDict.Keys)
            scores[key.ToString()] = (int)scoresDict[key].AsInt32();

        // Обновляем RecordHUD
        UIManager.Instance.UpdateScores(scores);
    }

    
    public void HandleServerMessage(string json)
    {
        var parsed = Json.ParseString(json);
        if (parsed.VariantType == Variant.Type.Nil)
            return;

        var message = parsed.AsGodotDictionary();
        var type = message.GetValueOrDefault("type", "").AsString();

        string playerId = "";

        Godot.Collections.Dictionary playerDict = null;

        if (message.TryGetValue("player", out var playerVar) &&
            playerVar.VariantType == Variant.Type.Dictionary)
        {
            playerDict = playerVar.AsGodotDictionary();
            playerId = playerDict.GetValueOrDefault("id", "").AsString();
        }
        else if (message.ContainsKey("playerId"))
        {
            playerId = message["playerId"].AsString();
        }
        
        Vector2 position = Vector2.Zero;
        Vector2 direction = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;
        string state = "";
        string nickname = "";

        if (playerDict != null)
        {
            position = new Vector2(
                (float)playerDict.GetValueOrDefault("x", 0).AsDouble(),
                (float)playerDict.GetValueOrDefault("y", 0).AsDouble()
            );

            direction = new Vector2(
                (float)playerDict.GetValueOrDefault("dirX", 0).AsDouble(),
                (float)playerDict.GetValueOrDefault("dirY", 0).AsDouble()
            );

            velocity = new Vector2(
                (float)playerDict.GetValueOrDefault("velX", 0).AsDouble(),
                (float)playerDict.GetValueOrDefault("velY", 0).AsDouble()
            );

            state = playerDict.GetValueOrDefault("currentState", "Idle").AsString();
            nickname = playerDict.GetValueOrDefault("nickname", "").AsString();
        }

        switch (type)
        {
            case "JOIN_ACK":
                HandleJoinAck(message);
                break;
            case "JOIN":
                GD.Print("Пакет JOIN:", json);
                PlayerManager?.OnPlayerJoined(playerId, position, nickname);
                break;
            case "MOVE":
                PlayerManager?.OnPlayerMoved(playerId, position, direction, velocity);
                break;
            case "LEAVE":
                // GD.Print("Пакет LEAVE:", json);
                PlayerManager?.OnPlayerLeave(playerId);
                break;
            case "STATE_CHANGED":
                // GD.Print("Пакет STATE:", json);
                PlayerManager?.OnPlayerStateChanged(playerId, state);
                break;
            case "BULLET_SPAWN":
                HandleBulletSpawn(message);
                break;
            case "BULLET_REMOVE":
                HandleBulletRemove(message);
                break;
            case "DAMAGE":
                // GD.Print("Пакет DAMAGE:", json);
                HandleDamage(message);
                break;
            case "PLAYER_RESPAWN":
                // GD.Print("Пакет RESPAWN:", json);
                HandleRespawn(message);
                break;
            case "GAME_TIME":
                GD.Print("Пакет GAME_TIME:", json);
                HandleGameTime(message);
                break;
            case "GAME_FINISHED":
                GD.Print("Пакет GAME_FINISHED:", json);
                HandleGameFinished();
                break;
            case "SCORE_UPDATE":
                HandleScoreUpdate(message);
                break;

        }
    }
    
    private void HandleRespawn(Godot.Collections.Dictionary msg)
    {
        var playerDict = msg["player"].AsGodotDictionary();

        var id = playerDict["id"].AsString();
        var x = (float)playerDict["x"].AsDouble();
        var y = (float)playerDict["y"].AsDouble();
        var hp = (float)playerDict["health"].AsDouble();

        var player = PlayerManager.GetPlayer(id);
        player?.RespawnPlayer(new Vector2(x, y), hp);
    }

    // private void HandleDamage(Godot.Collections.Dictionary msg)
    // {
    //     var dmg = (int)msg["dmg"];
    //     
    //     var bullet = msg["bullet"].AsGodotDictionary();
    //     
    //     var targetId = bullet["targetId"].AsString();
    //
    //     var player = PlayerManager.GetPlayer(targetId);
    //     if (player == null)
    //         return;
    //
    //     player.TakeDamage(dmg);
    // }
    
    private void HandleDamage(Godot.Collections.Dictionary msg)
    {
        var bullet = msg["bullet"].AsGodotDictionary();
        var targetId = bullet["targetId"].AsString();

        var dmg = (int)msg["dmg"];
        var player = PlayerManager.GetPlayer(targetId);
        player?.TakeDamage(dmg);
    }
    
    private void HandleBulletSpawn(Godot.Collections.Dictionary msg)
    {
        if (!msg.ContainsKey("bullet"))
            return;

        var b = msg["bullet"].AsGodotDictionary();

        var id = b.GetValueOrDefault("id", "").AsString();
        var owner = b.GetValueOrDefault("ownerId", "").AsString();

        var x = (float)b.GetValueOrDefault("x", 0f).AsDouble();
        var y = (float)b.GetValueOrDefault("y", 0f).AsDouble();

        var dx = (float)b.GetValueOrDefault("dirX", 0f).AsDouble();
        var dy = (float)b.GetValueOrDefault("dirY", 0f).AsDouble();

        _bulletManager.Spawn( id, owner, new Vector2(x, y), new Vector2(dx, dy));
    }
    
    private void HandleBulletRemove(Godot.Collections.Dictionary msg)
    {
        if (!msg.ContainsKey("bullet"))
            return;

        var b = msg["bullet"].AsGodotDictionary();
        var id = b.GetValueOrDefault("id", "").AsString();

        _bulletManager.Remove(id);
    }
    
    private void HandleJoinAck(Godot.Collections.Dictionary msg)
    {
        // if (msg.ContainsKey("existingBullets"))
        // {
        //     foreach (Variant entry in (Godot.Collections.Array)msg["existingBullets"])
        //     {
        //         var dict = entry.AsGodotDictionary();
        //         var bulletDto = ParseBullet(dict);
        //
        //         _bulletManager.SpawnBullet(
        //             bulletDto.Id,
        //             bulletDto.OwnerId,
        //             new Vector2(bulletDto.X, bulletDto.Y),
        //             new Vector2(bulletDto.DirX, bulletDto.DirY)
        //         );
        //     }
        // }
        
        if (msg == null)
            return;

        LocalUserID = msg["playerId"].AsString();

        var pos = Vector2.Zero;

        var playerDict = msg.GetValueOrDefault("player", "").AsGodotDictionary();
        if (playerDict != null)
        {
            pos = new Vector2((float)playerDict.GetValueOrDefault("x", 0).AsDouble(),
                (float)playerDict.GetValueOrDefault("y", 0).AsDouble());
        }

        PlayerManager.SpawnLocalPlayer(LocalUserID, pos);

        if (!msg.TryGetValue("existingPlayers", out var value)) 
            return;

        foreach (var entry in (Godot.Collections.Array)value)
        {
            var dict = entry.AsGodotDictionary();
            var otherId = dict.GetValueOrDefault("id", "").AsString();
            var otherPos = new Vector2(
                (float)dict.GetValueOrDefault("x", 0).AsDouble(),
                (float)dict.GetValueOrDefault("y", 0).AsDouble()
            );
            
            var nickname = dict.GetValueOrDefault("nickname", "").AsString();
            
            // GD.Print(nickname);
            PlayerManager.OnPlayerJoined(otherId, otherPos, nickname);
        }
    }

    
    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest)
            return;

        if (!OfflineMode && IsOpen)
        {
            SendLeaveRequest(LocalUserID);

            // Даем шанс отправить пакет
            _webSocket.Poll();
            _webSocket.Close();
        }

        GetTree().Quit();
    }
}
