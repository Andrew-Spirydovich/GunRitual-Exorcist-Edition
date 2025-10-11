using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player) => AnimationName = "Idle";
    
    public override void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (Player.InputVector != Vector2.Zero)
                return new RunState(Player);

            if (controlContext.IsJumpPressed && Player.IsOnFloor())
                return new JumpState(Player);

            if (controlContext.IsRollPressed && Player.IsOnFloor())
                return new RollState(Player);

            if (!Player.IsOnFloor() && Player.Velocity.Y > 0)
                return new FallState(Player);

            if (controlContext.IsSlidePressed && Player.IsOnFloor())
                return new SlideState(Player);
            
            if (controlContext.IsFirePressed)
            {
                return Player.Attack();
            }

            if (controlContext.IsReloadPressed)
            {
                Player.Reload();
            }
        }
        
        return null;
    }
}
