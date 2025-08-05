using Godot;
using System;

public partial class Player : Node2D
{
    [Export]
    private CollisionShape2D _collider;

    private PlayerMovement _movement;

    public override void _Ready()
    {
        _movement = new PlayerMovement(_collider);
    }

    public override void _PhysicsProcess(double delta)
    {
        _movement.Move(this, delta);
    }
}
