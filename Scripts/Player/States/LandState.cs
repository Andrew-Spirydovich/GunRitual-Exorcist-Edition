using Godot;
using System;

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
        
        // Когда анимация закончится → смена стейта
        _player.Animator.ConnectAnimationFinished(OnAnimationFinished); 
    }

    public override void Exit()
    {
        _player.Animator.DisconnectAnimationFinished(OnAnimationFinished); 
    }

    public override void Update(double delta)
    {
        if (_player.InputVector != Vector2.Zero)
        {
            _player.StateMachine.ChangeState(new RunState(_player));
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector);
    }
    
    private void OnAnimationFinished()
    {
        if (_player.InputVector != Vector2.Zero)
            _player.StateMachine.ChangeState(new RunState(_player));
        else
            _player.StateMachine.ChangeState(new IdleState(_player));
    }
}
