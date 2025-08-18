using Godot;
using System;

public class JumpState : PlayerState
{
    private Player _player;
    private const float JUMP_FORCE = 400f;

    public JumpState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        var vel = _player.Velocity;
        vel.Y = -JUMP_FORCE;
        _player.Velocity = vel;
        
        _player.Animator.SetAnimation("Jump");
    }   

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.Velocity.Y > 0) // Начал падать
        {
            _player.StateMachine.ChangeState(new FallState(_player));
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        _player.Movement.HandleHorizontalMovement(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector);
    }
}
