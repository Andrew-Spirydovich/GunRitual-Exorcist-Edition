using Godot;
using System;

public partial class CameraController : Camera2D
{
    [Export] private Node2D _target;

    public override void _Process(double delta)
    {
        if (_target != null)
        {
            GlobalPosition = _target.GlobalPosition;
        }
    }
}
