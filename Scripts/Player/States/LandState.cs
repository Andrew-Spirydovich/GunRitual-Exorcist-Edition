using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class LandState : PlayerState
{
    public LandState(Player player) : base(player) => AnimationName = "Land";

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
            if (_player.InputVector != Vector2.Zero)
                _player.StateMachine.ChangeState(PlayerStateType.Run);
            
            if (Input.IsActionJustPressed("input_roll"))
                _player.StateMachine.ChangeState(PlayerStateType.Roll);
            
            if (WantsToSlide() && _player.Movement.IsOnFloor())
                _player.StateMachine.ChangeState(PlayerStateType.Slide);
            
            if (Input.IsActionJustPressed("input_fire"))
            {
                var weapon = _player.Inventory.CurrentWeapon;
                
                if (weapon != null && weapon.CurrentAmmo > 0)
                    _player.StateMachine.ChangeState(PlayerStateType.Shoot);
            }
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
