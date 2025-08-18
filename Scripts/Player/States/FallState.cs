using Godot;
using System;

public class FallState : PlayerState
{
    private Player _player;
    private const float JUMP_FORCE = 400f;

    public FallState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("Fall");
    }   

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.IsOnFloor())
        {
            _player.StateMachine.ChangeState(new LandState(_player));
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
