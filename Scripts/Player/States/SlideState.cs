using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class SlideState : PlayerState
{
    private double _timer = 0.5;
    public SlideState(Player player) : base(player) => AnimationName = "Slide";

    public override void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);
        Player.Movement.HandeSlide();
    }

    public override void Exit()
    {
        Player.Velocity = Vector2.Zero;
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        Player.Movement.ApplyGravity(delta);

        _timer -= delta;
        Player.Velocity *= 0.95f;
        Player.MoveAndSlide();

        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (_timer <= 0)
            {
                if (Player.InputVector.X != 0)
                    return new RunState(Player);
                
                return new IdleState(Player);
            }
            
            if (controlContext.IsJumpPressed && Player.IsOnFloor())
                return new JumpState(Player);

            if (Player.Velocity.Y > 0)
                return new FallState(Player);

            if (controlContext.IsFirePressed)
            {
                var weapon = Player.Inventory.CurrentWeapon;

                if (weapon != null && weapon.CurrentAmmo > 0)
                    return new ShootState(Player);
            }
        }

        return null;
    }
}