using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class RollState : PlayerState
{
    public RollState(Player player) : base(player)
    {
        AnimationName = "Roll";
        WaitsForAnimationEnd = true;
    }

    public override void PhysicsUpdate(double delta)
    {
        Player.Movement.ApplyGravity(delta);
        Player.Movement.HandleRoll();

        Player.MoveAndSlide();

        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (IsAnimationDone())
                return new IdleState(Player);
            
            if (Player.Velocity.Y > 0)
                return new FallState(Player);
            
        }

        return null;
    }
}
