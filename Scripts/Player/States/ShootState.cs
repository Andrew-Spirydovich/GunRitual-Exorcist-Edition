using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public class ShootState : PlayerState
{
    public ShootState(Player player) : base(player)
    {
        AnimationName = "ShootPistol";
        WaitsForAnimationEnd = true;
    }

    public override void Enter()
    {
        Player.Velocity = Vector2.Zero;
        base.Enter();
    }

    public override void Exit()
    {
        AnimationForceFinish();
    }
    
    public override void PhysicsUpdate(double delta)
    {
        Player.Movement.ApplyGravity(delta);
        Player.MoveAndSlide();
        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Vector2.Zero, Vector2.Zero);
    }
    
    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        if (Player.IsLocal)
        {
            if (controlContext.IsFirePressed)
                return Player.Attack();
            
            if(controlContext.IsReloadPressed)
                Player.Reload();
            
            if (Player.Velocity.Y > 250)
                return new FallState(Player);
            
            if (IsAnimationDone())
                return new IdleState(Player);

        }
        return null;
    }
}
