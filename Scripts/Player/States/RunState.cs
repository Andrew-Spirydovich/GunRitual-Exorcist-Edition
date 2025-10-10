using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class RunState : PlayerState
{
    public RunState(Player player) : base(player) => AnimationName = "Run";

    public override void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        Player.Movement.ApplyGravity(delta);
        Player.Movement.HandleHorizontalMovement();

        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (Player.InputVector == Vector2.Zero)
                return new IdleState(Player);

            if (controlContext.IsJumpPressed && Player.IsOnFloor())
                return new JumpState(Player);

            if (controlContext.IsRollPressed)
                return new RollState(Player);

            if (Player.Velocity.Y > 0)
                return new FallState(Player);

            if (controlContext.IsSlidePressed && Player.Movement.IsOnFloor())
                return new SlideState(Player);

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