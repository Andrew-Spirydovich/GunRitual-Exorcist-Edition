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
    }
    
    public override State<Character> CheckTransitions(InputContext context)
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
            { 
                return attacker.Attack();
            }
            
            if (Entity is not IArmedAttacker armedAttacker) 
                return null;

            if (control.IsReloadPressed)
            {
                armedAttacker.Reload();
            }

        }
        else if (Entity.ControlMode == ControlMode.AI)
        {
            if (IsAnimationDone())
            {
                if (Entity.InputVector != Vector2.Zero)
                    return new RunState(Entity);
                else
                    return new IdleState(Entity);
            }
        }
        return null;
    }
}
