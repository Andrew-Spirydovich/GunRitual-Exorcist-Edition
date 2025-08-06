using Godot;
using System;

public partial class NetworkManager : Node
{
    public override void _Ready()
    {
        GD.Print("Network Manager готов");
    }

    public void ConnectToSession(string ip, int port)
    {
        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateClient(ip, port);
        
        if (result != Error.Ok)
        {
            GD.PrintErr("Не удалось подключиться к серверу: " + result);
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Клиент подключен!");
    }

    public void HostSession(int port)
    {
        var peer = new ENetMultiplayerPeer();
        var result = peer.CreateServer(port);
        
        if (result != Error.Ok)
        {
            GD.PrintErr("Не удалось создать сервер: " + result);
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Сервер запущен!");
    }
}
