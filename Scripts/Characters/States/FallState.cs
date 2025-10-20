using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class FallState : State<Character>
{
    public FallState(Character entity) : base(entity) => AnimationName = "Fall";
    
    public override State<Character> CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
            if (Entity.IsOnFloor())
                return new LandState(Entity);

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