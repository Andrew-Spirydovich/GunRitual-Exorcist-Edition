using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class RollState : PlayerState
{
    public RollState(Player player) : base(player) => AnimationName = "Roll";

    public override void Enter()
    {
        _player.Animator.SetAnimation(AnimationName);
        
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
                _player.StateMachine.ChangeState(PlayerStateType.Fall);
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        _player.Movement.ApplyGravity(delta);

        _player.Movement.HandleRoll();

        _player.MoveAndSlide();

        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }

}
