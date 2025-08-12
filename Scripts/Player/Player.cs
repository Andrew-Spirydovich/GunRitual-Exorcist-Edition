using Godot;
using System;
using System.Globalization;

public partial class Player : Node2D
{
    [Export]
    private CollisionShape2D _collider;
    [Export]
    private AnimatedSprite2D _sprite;
    [Export]
    private Camera2D _camera;
    
    public bool IsLocal { get; set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerMovement Movement { get; private set; }
    
    public Vector2 InputVector => Movement.GetInputDirection();
    
    public override void _Ready()
    {
        Movement = new PlayerMovement(_collider, this);
        Animator = new PlayerAnimator(_sprite);
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));
        SetProcess(true);
        
        if (!IsLocal)
            return;
        _camera.Enabled = true;
    }

    public override void _Process(double delta)
    {
        StateMachine?.Update(delta);
        Animator?.UpdateDirection(InputVector);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        StateMachine?.PhysicsUpdate(delta);
    }
}
