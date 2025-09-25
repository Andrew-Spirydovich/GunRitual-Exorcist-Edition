using Godot;
using System;

public abstract class PlayerState
{
    protected readonly Player _player;
    protected abstract string AnimationName { get; }
    protected PlayerState(Player player)
    {
        _player = player;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
    
    public bool WantsToSlide()
    {
        return Input.IsActionPressed("input_down") &&
               (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
               Input.IsActionJustPressed("input_down");
    }
}
