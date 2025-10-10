using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class JumpState : PlayerState
{
    private const float JUMP_FORCE = 400f;
    public JumpState(Player player) : base(player) => AnimationName = "Jump";

    public override void Enter()
    {
        var vel = Player.Velocity;
        vel.Y = -JUMP_FORCE;
        Player.Velocity = vel;
        Player.Animator.SetAnimation(AnimationName);
    }   

    public override void Exit() { }
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
