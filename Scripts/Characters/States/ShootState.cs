using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class ShootState : State<Character>
{
    public ShootState(Character entity) : base(entity)
    {
        AnimationName = "ShootPistol";
        WaitsForAnimationEnd = true;
    }

    public override void Enter()
    {
        Entity.Velocity = Vector2.Zero;
        base.Enter();
    }

    public override void Exit()
    {
        AnimationForceFinish();
    }
    
    public override void PhysicsUpdate(double delta)
    {
        Entity.MovementController.ApplyGravity(delta);
        Entity.MoveAndSlide();
        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Entity.GlobalPosition, Vector2.Zero, Vector2.Zero);
    }
    
    public override State<Character>  CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
            if (Entity.Velocity.Y > 250)
                return new FallState(Entity);
            
            if (IsAnimationDone())
                return new IdleState(Entity);
            
            if (Entity is not IAttacker attacker) 
                return null;
            
            if (control.IsFirePressed)
                return attacker.Attack();
            
            if(control.IsReloadPressed)
                attacker.Reload();

        }
        return null;
    }
}
