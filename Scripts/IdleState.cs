using Godot;
using System;

public class IdleState : PlayerState
{
    private Player _player;

    public IdleState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animation.Play("Idle");
    }

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.InputVector != Vector2.Zero)
        {
            _player.StateMachine.ChangeState(new RunState(_player));
        }
    }

    public override void PhysicsUpdate(double delta) { }
}
