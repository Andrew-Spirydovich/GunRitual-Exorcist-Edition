using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class JumpState : PlayerState
{
    public JumpState(Player player) : base(player) => AnimationName = "Jump";

    public override void Enter()
    {
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
