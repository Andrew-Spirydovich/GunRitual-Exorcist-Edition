using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

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
        if (_player.IsLocal) // локальный игрок — проверяем input
        {
            if (_player.InputVector == Vector2.Zero)
                _player.StateMachine.ChangeState(PlayerStateType.Idle);

            if (Input.IsActionJustPressed("input_jump") && _player.IsOnFloor())
                _player.StateMachine.ChangeState(PlayerStateType.Jump);

            if (Input.IsActionJustPressed("input_roll"))
                _player.StateMachine.ChangeState(PlayerStateType.Roll);

            if (_player.Velocity.Y > 0)
                _player.StateMachine.ChangeState(PlayerStateType.Fall);

            if (WantsToSlide() && _player.Movement.IsOnFloor())
                _player.StateMachine.ChangeState(PlayerStateType.Slide);
        }
    }
    
    public bool WantsToSlide()
    {
        return Input.IsActionPressed("input_down") &&
               (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
               Input.IsActionJustPressed("input_down");
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        _player.Movement.HandleHorizontalMovement(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
