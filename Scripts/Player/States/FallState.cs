using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class FallState : PlayerState
{
    public FallState(Player player) : base(player) => AnimationName = "Fall";

    public override void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (Player.IsOnFloor())
                return new LandState(Player);

            if (controlContext.IsFirePressed)
            {
                return Player.Attack();
            }
        }

        return null;
    }
}