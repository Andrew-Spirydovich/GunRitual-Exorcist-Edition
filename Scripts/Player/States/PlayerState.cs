using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public abstract class PlayerState
{
    protected readonly Player Player;
    protected string AnimationName;
    protected bool WaitsForAnimationEnd;
    private bool _animationFinished;
    
    protected PlayerState(Player player)
    {
        Player = player;
    }

    public virtual void Enter()
    {
        Player.Animator.SetAnimation(AnimationName);

        if (WaitsForAnimationEnd)
        {
            _animationFinished = false;
            Player.Animator.ConnectAnimationFinished(OnAnimationFinished);
        }
    }

    public virtual void Update(double delta) { }

    public virtual void PhysicsUpdate(double delta)
    {
        Player.Movement.ApplyGravity(delta);
        Player.Movement.HandleHorizontalMovement();
        
        var network = NetworkClient.Instance;
        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Vector2.Zero, Vector2.Zero);
    }
    public abstract PlayerState CheckTransitions(ControlContext controlContext);

    private void OnAnimationFinished()
    {
        _animationFinished = true;
    }

    protected void AnimationForceFinish()
    {
        Player.Animator.SetAnimation("Idle");
        Player.Animator.DisconnectAnimationFinished(OnAnimationFinished);
        _animationFinished = true;
    }
    
    protected bool IsAnimationDone() => _animationFinished;
    public virtual void Exit()
    {
        if (WaitsForAnimationEnd)
            Player.Animator.DisconnectAnimationFinished(OnAnimationFinished);
    }
    
}
