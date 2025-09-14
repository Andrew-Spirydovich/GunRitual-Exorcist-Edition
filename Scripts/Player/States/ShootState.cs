using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class ShootState : PlayerState
{
    private Player _player;
    private double _shootCooldown;
    
    public ShootState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("ShootPistol");
        var weapon = _player.Inventory.CurrentWeapon;
        
        _player.Attack();
        
        _shootCooldown = weapon.FireRate;
    }

    public override void Exit()
    {
        _player.Velocity = Vector2.Zero;
    }

    public override void Update(double delta)
    {
        if (_shootCooldown > 0)
        {
            _shootCooldown -= delta;
            return; // ещё нельзя менять стейт
        }
        
        if (_player.IsLocal) // локальный игрок — проверяем input
        {
            if (_player.InputVector == Vector2.Zero)
                _player.StateMachine.ChangeState(PlayerStateType.Idle);
            
            if (_player.InputVector != Vector2.Zero)
                _player.StateMachine.ChangeState(PlayerStateType.Run);

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
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector, _player.Velocity);
    }
}
