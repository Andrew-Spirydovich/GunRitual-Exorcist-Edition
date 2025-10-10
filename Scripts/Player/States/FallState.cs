using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class FallState : PlayerState
{
    public FallState(Player player) : base(player) => AnimationName = "Fall";

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
            if (Player.IsOnFloor())
                return new LandState(Player);

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