using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

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


    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
    public abstract PlayerState CheckTransitions(ControlContext controlContext);

    private void OnAnimationFinished()
    {
        _animationFinished = true;
    }
    
    protected bool IsAnimationDone() => _animationFinished;
    public virtual void Exit()
    {
        if (WaitsForAnimationEnd)
            Player.Animator.DisconnectAnimationFinished(OnAnimationFinished);
    }
    
}
