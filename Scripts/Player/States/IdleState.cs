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
        _player.Animator.SetAnimation("Idle");
    }

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.InputVector != Vector2.Zero)
        {
            _player.StateMachine.ChangeState(new RunState(_player));
        }
        
        if (Input.IsActionJustPressed("input_jump") && _player.IsOnFloor())
        {
            _player.StateMachine.ChangeState(new JumpState(_player));
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        _player.Movement.HandleHorizontalMovement(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, Vector2.Zero);
    }
}
