using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class RunState : State<Character>
{
    public RunState(Character entity) : base(entity) => AnimationName = "Run";

    public override void Enter()
    {
        UpdateAnimation();
    }
    
    private void UpdateAnimation()
    {
        if (Entity is not IArmedAttacker attacker)
            return;
        
        var weaponSuffix = attacker.CurrentWeapon.Type switch
        {
            WeaponType.Revolver => "Pistol",
            WeaponType.Ithaca37 => "Pistol",
            _ => ""
        };
        
        Entity.Animator.SetAnimation(AnimationName+weaponSuffix);
    }

    public override State<Character> CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
            if (Entity.InputVector == Vector2.Zero)
                return new IdleState(Entity);

            if (control.IsJumpPressed && Entity.IsOnFloor())
                return new JumpState(Entity);

            if (control.IsRollPressed && Entity.IsOnFloor())
                return new RollState(Entity);

            if (Entity.Velocity.Y > 0)
                return new FallState(Entity);

            if (control.IsSlidePressed && Entity.IsOnFloor())
                return new SlideState(Entity);

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
            var ai = context as AIContext;
            if (ai == null) return null;
            
            if (ai.MoveDirection == Vector2.Zero)
                return new IdleState(Entity);
            
            if (ai.ShouldFire && Entity is IAttacker attacker)
                return attacker.Attack();
            
            if (ai.ShouldJump && Entity.IsOnFloor())
                return new JumpState(Entity);
        }


        return null;
    }
}