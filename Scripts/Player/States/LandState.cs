using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class LandState : PlayerState
{
    private Player _player;

    public LandState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("Land");
        _player.Animator.ConnectAnimationFinished(OnAnimationFinished); 
    }

    public override void Exit()
    {
        _player.Animator.DisconnectAnimationFinished(OnAnimationFinished); 
    }

    public override void Update(double delta)
    {
        if (_player.IsLocal)
        {
            if (_player.InputVector != Vector2.Zero)
            {
                _player.StateMachine.ChangeState(PlayerStateType.Run);
            }
            
            if (Input.IsActionJustPressed("input_roll"))
            {
                _player.StateMachine.ChangeState(PlayerStateType.Roll);
            }
            
            if (WantsToSlide() && _player.Movement.IsOnFloor())
            {
                _player.StateMachine.ChangeState(PlayerStateType.Slide);
            }
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
    
    public bool WantsToSlide()
    {
        return Input.IsActionPressed("input_down") &&
               (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
               Input.IsActionJustPressed("input_down");
    }
    
    private void OnAnimationFinished()
    {
        if (_player.InputVector != Vector2.Zero)
            _player.StateMachine.ChangeState(PlayerStateType.Run);
        else
            _player.StateMachine.ChangeState(PlayerStateType.Idle);
    }
}
