using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class JumpState : PlayerState
{
    private AudioStreamMP3 _jumpSound;
    public JumpState(Player player) : base(player)
    {
        AnimationName = "Jump";
        _jumpSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/player jump.mp3");
    }

    public override void Enter()
    {
        AudioManager.Instance.PlaySFX(_jumpSound);
        Player.Movement.HandeJump();
        Player.Animator.SetAnimation(AnimationName);
    }   

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (Player.Velocity.Y > 0)
                return new FallState(Player);

            if (controlContext.IsFirePressed)
            {
                return Player.Attack();
            }
        }

        return null;
    }
}
