using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class SlideState : State<Character>
{
    private double _timer = 0.5;
    public SlideState(Character entity) : base(entity) => AnimationName = "Slide";

    public override void Enter()
    {
        base.Enter();
        Entity.MovementController.HandeSlide();
    }

    public override void Exit()
    {
        Entity.Velocity = Vector2.Zero;
    }

    public override void PhysicsUpdate(double delta)
    {
        Entity.MovementController.ApplyGravity(delta);

        _timer -= delta;
        Entity.Velocity *= 0.95f;
        Entity.MoveAndSlide();
    }

    public override State<Character> CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
            if (_timer <= 0)
            {
                if (Entity.InputVector.X != 0)
                    return new RunState(Entity);
                
                return new IdleState(Entity);
            }
            
            if (control.IsJumpPressed && Entity.IsOnFloor())
                return new JumpState(Entity);

            if (Entity.Velocity.Y > 0)
                return new FallState(Entity);

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