using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class RollState : PlayerState
{
    private Player _player;
    private Vector2 _rollDirection;
    private float _rollSpeed = 300f;
    
    public RollState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("Roll");
        
        _rollDirection = _player.InputVector != Vector2.Zero 
            ? _player.InputVector.Normalized()
            : new Vector2(_player.Movement.FacingRight ? 1 : -1, 0);
        
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
            if (_player.Velocity.Y > 0)
            {
                _player.StateMachine.ChangeState(PlayerStateType.Fall);
            }
        }
    }

    public override void PhysicsUpdate(double delta)
    { 
        _player.Movement.ApplyGravity(delta);

        Vector2 velocity = _rollDirection * _rollSpeed;
        _player.Velocity = new Vector2(velocity.X, _player.Velocity.Y);

        _player.MoveAndSlide();
        
        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
    
    private void OnAnimationFinished()
    {
        if (_player.InputVector != Vector2.Zero)
            _player.StateMachine.ChangeState(PlayerStateType.Run);
        else
            _player.StateMachine.ChangeState(PlayerStateType.Idle);
    }
}
