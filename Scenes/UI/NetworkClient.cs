using Godot;
using System;

public partial class NetworkClient : Node
{
    private WebSocketPeer _peer = new WebSocketPeer();

    public override void _Ready()
    {
        var err = _peer.ConnectToUrl("ws://192.168.31.73:8080/ws");
        
        if (err != Error.Ok)
            GD.PrintErr("Ошибка подключения к серверу: " + err);
    }

    public override void _Process(double delta)
    {
        _peer.Poll();

        while (_peer.GetAvailablePacketCount() > 0)
        {
            var msg = _peer.GetPacket().GetStringFromUtf8();
            GD.Print("Сообщение от сервера: " + msg);
        }
    }

    public void SendMessage(string msg)
    {
        _peer.SendText(msg);
    }
}
