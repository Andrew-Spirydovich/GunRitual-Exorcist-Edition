using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public abstract class State<T> where T : Character
{
    protected readonly T Entity;
    private readonly Animator _animator;
    private readonly MovementController _movementController;
    
    protected string AnimationName;
    protected bool WaitsForAnimationEnd;
    private bool _animationFinished;
    
    protected State(T entity)
    {
        Entity = entity;
        _animator = entity.Animator;
        _movementController = entity.MovementController;
    }

    public virtual void Enter()
    {
        if(!string.IsNullOrEmpty(AnimationName))
            _animator.SetAnimation(AnimationName);
        
        if (WaitsForAnimationEnd)
        {
            _animationFinished = false;
            _animator.AnimationFinished += OnAnimationFinished;
        }
    }

    public virtual void PhysicsUpdate(double delta)
    { 
        if (Entity.ControlMode == ControlMode.Local)
        {
            _movementController.HandleHorizontalMovement();
            var network = NetworkClient.Instance;
            network.SendMoveRequest(network.LocalUserID, _movementController.GlobalPosition, Vector2.Zero, Vector2.Zero);
        }
        
        _movementController.ApplyGravity(delta);
    }
    public abstract State<T> CheckTransitions(InputContext controlContext);

    private void OnAnimationFinished()
    {
        _animationFinished = true;
    }

    protected void AnimationForceFinish()
    {
        _animator.SetAnimation("Idle");
        _animator.AnimationFinished -= OnAnimationFinished;
        _animationFinished = true;
    }
    
    protected bool IsAnimationDone() => _animationFinished;
    public virtual void Exit()
    {
        if (WaitsForAnimationEnd)
            _animator.AnimationFinished -= OnAnimationFinished;
    }
    
}
