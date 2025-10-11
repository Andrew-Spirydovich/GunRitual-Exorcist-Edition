using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public class LandState : PlayerState
{
    public LandState(Player player) : base(player)
    {
        AnimationName = "Land";
        WaitsForAnimationEnd = true;
    }

    public override void Exit()
    {
        Player.Velocity = Vector2.Zero;
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        Player.Movement.ApplyGravity(delta);

        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
                        
            if (IsAnimationDone())
                return new IdleState(Player);
            
            if (controlContext.IsJumpPressed && Player.IsOnFloor())
                return new JumpState(Player);
            
            if (Player.InputVector != Vector2.Zero)
                return new RunState(Player);

            if (controlContext.IsRollPressed && Player.IsOnFloor())
                return new RollState(Player);

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