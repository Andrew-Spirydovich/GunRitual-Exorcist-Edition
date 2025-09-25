using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class FallState : PlayerState
{
    protected override string AnimationName { get; }
    
    public FallState(Player player) : base(player) => AnimationName = "Fall";
    
    public override void Enter()
    {
        _player.Animator.SetAnimation(AnimationName);
    }   

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.IsLocal)
        {
            if (_player.IsOnFloor())
                _player.StateMachine.ChangeState(PlayerStateType.Land);
            
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
        _player.Movement.HandleHorizontalMovement(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
