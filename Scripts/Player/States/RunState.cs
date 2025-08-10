using Godot;
using System;

public class RunState : PlayerState
{
    private Player _player;

    public RunState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("Run");
    }

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.InputVector == Vector2.Zero)
        {
            _player.StateMachine.ChangeState(new IdleState(_player));
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _player.Movement.Move(_player, delta);
        NetworkClient.Instance.SendMove("andrew", _player.GlobalPosition);
    }
}
