using Godot;
using System;

public partial class Player : Node2D
{
    [Export]
    private CollisionShape2D _collider;
    [Export]
    private AnimatedSprite2D _sprite;
    
    public StateMachine StateMachine { get; private set; }
    public PlayerAnimation Animation { get; private set; }
    public PlayerMovement Movement { get; private set; }
    
    public Vector2 InputVector => Movement.GetInputDirection();
    
    public override void _Ready()
    {
        Movement = new PlayerMovement(_collider);
        Animation = new PlayerAnimation(_sprite);
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
    }

    public override void _Process(double delta)
    {
        StateMachine.Update(delta);
        Animation.UpdateDirection(InputVector);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        StateMachine.PhysicsUpdate(delta);
    }
}
