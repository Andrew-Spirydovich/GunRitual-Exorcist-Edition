using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class LandState : State<Character>
{
    public LandState(Character entity) : base(entity)
    {
        AnimationName = "Land";
        WaitsForAnimationEnd = true;
    }

    public override void Exit()
    {
        Entity.Velocity = Vector2.Zero;
    }

    public override void PhysicsUpdate(double delta)
    {
        Entity.UpdateGravity(delta);
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
            
            if (control.IsJumpPressed && Entity.IsOnFloor())
                return new JumpState(Entity);
            
            if (Entity.InputVector != Vector2.Zero)
                return new RunState(Entity);

            if (control.IsRollPressed && Entity.IsOnFloor())
                return new RollState(Entity);

            if (control.IsSlidePressed && Entity.IsOnFloor())
                return new SlideState(Entity);

            if (Entity is not IAttacker attacker) 
                return null;
            
            if (control.IsFirePressed)
            { 
                return attacker.Attack();
            }
        }

        return null;
    }
}