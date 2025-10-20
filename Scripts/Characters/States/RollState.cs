using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class RollState : State<Character>
{
    public RollState(Character entity) : base(entity)
    {
        AnimationName = "Roll";
        WaitsForAnimationEnd = true;
    }

    public override void PhysicsUpdate(double delta)
    {
        Entity.MovementController.ApplyGravity(delta);
        Entity.MovementController.HandleRoll();

        Entity.MoveAndSlide();

        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Entity.GlobalPosition, Entity.InputVector, Entity.Velocity);
    }

    public override State<Character> CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
            if (IsAnimationDone())
                return new IdleState(Entity);
            
            if (Entity.Velocity.Y > 0)
                return new FallState(Entity);
            
        }

        return null;
    }
}
