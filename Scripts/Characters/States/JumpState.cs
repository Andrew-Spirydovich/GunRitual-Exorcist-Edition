using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class JumpState : State<Character>
{
    private AudioStreamMP3 _jumpSound;
    public JumpState(Character entity) : base(entity)
    {
        AnimationName = "Jump";
        _jumpSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/player jump.mp3");
    }

    public override void Enter()
    {
        AudioManager.Instance.PlaySFX(_jumpSound);
        Entity.Jump();
        base.Enter();
    }   

    public override State<Character> CheckTransitions(InputContext context)
    {
        if (Entity.ControlMode == ControlMode.Local)
        {
            var control = context as ControlContext;
            
            if (control == null)
                return null;
            
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
