using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class SlideState : PlayerState
{
    private Player _player;
    private Vector2 _slideDirection;
    private double _slideDuration = 0.5;
    private double _timer;
    private float _slideSpeed = 400f;
    
    public SlideState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _slideDirection = new Vector2(_player.Movement.FacingRight ? 1 : -1, 0);

        _timer = _slideDuration;

        _player.Animator.SetAnimation("Slide");
        
        _player.Velocity = _slideDirection * _slideSpeed;
    }

    public override void Exit()
    {
        _player.Velocity = Vector2.Zero;
    }

    public override void Update(double delta)
    {
        if (_player.IsLocal)
        {
            if (Input.IsActionJustPressed("input_jump") && _player.IsOnFloor())
            {
                _player.StateMachine.ChangeState(PlayerStateType.Jump);
            }
        
            if (_player.Velocity.Y > 0)
            {
                _player.StateMachine.ChangeState(PlayerStateType.Fall);
            }
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _player.Movement.ApplyGravity(delta);
        
        _timer -= delta;
        _player.Velocity *= 0.95f;
        _player.MoveAndSlide();
        
        if (_timer <= 0)
        {
            if (_player.InputVector.X != 0)
                _player.StateMachine.ChangeState(PlayerStateType.Run);
            else
                _player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
        
        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
