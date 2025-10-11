using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public class RunState : PlayerState
{
    public RunState(Player player) : base(player) => AnimationName = "Run";

    public override void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if(controlContext.IsReloadPressed)
                Player.Reload();
            
            if (Player.InputVector == Vector2.Zero)
                return new IdleState(Player);

            if (controlContext.IsJumpPressed && Player.IsOnFloor())
                return new JumpState(Player);

            if (controlContext.IsRollPressed && Player.IsOnFloor())
                return new RollState(Player);

            if (Player.Velocity.Y > 0)
                return new FallState(Player);

            if (controlContext.IsSlidePressed && Player.IsOnFloor())
                return new SlideState(Player);

            if (controlContext.IsFirePressed)
            {
                return Player.Attack();
            }
        }


        return null;
    }
}