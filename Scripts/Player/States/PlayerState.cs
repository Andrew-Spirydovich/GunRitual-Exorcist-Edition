using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public abstract class PlayerState
{
    protected readonly Player _player;
    protected string AnimationName { get; set; }
    
    protected PlayerState(Player player)
    {
        _player = player;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);

    protected bool WantsToSlide()
    {
        return Input.IsActionPressed("input_down") &&
               (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
               Input.IsActionJustPressed("input_down");
    }

    protected void OnAnimationFinished()
    {
        if (_player.InputVector != Vector2.Zero)
            _player.StateMachine.ChangeState(PlayerStateType.Run);
        else
            _player.StateMachine.ChangeState(PlayerStateType.Idle);
    }

}
