using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class RunState : PlayerState
{
    public RunState(Player player) : base(player) => AnimationName = "Run";
    
    public override void Enter()
    {
        _player.Animator.SetAnimation(AnimationName);
    }   

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.IsLocal)
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
        _player.Movement.HandleHorizontalMovement();
        
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
