using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class LandState : PlayerState
{
    public LandState(Player player) : base(player)
    {
        AnimationName = "Land";
        WaitsForAnimationEnd = true;
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

        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
                        
            if (IsAnimationDone())
                return new IdleState(Player);
            
            if (Player.InputVector != Vector2.Zero)
                return new RunState(Player);

            if (controlContext.IsRollPressed)
                return new RollState(Player);

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