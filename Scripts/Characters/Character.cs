using Godot;
using GunRitualExorcistEdition.Scripts.Core;

namespace GunRitualExorcistEdition.Scripts.Characters;

public enum ControlMode {Local, AI, Remote}

public abstract partial class Character : CharacterBody2D
{
    [Export] private AnimatedSprite2D _sprite;
    [Export] private PackedScene _bloodEffects;
    [Export] private CollisionShape2D _collider;
    [Export] private Material _materialOutline;
    [Export] protected Marker2D AttackMarker;
    public ControlMode ControlMode { get; private set; } = ControlMode.Local;
    
    public Animator Animator { get; private set; }
    public StateMachine<Character> StateMachine { get; private set; }
    public MovementController MovementController { get; private set; }

    protected float Health = 100f;
    public Vector2 InputVector { get; protected set; }
    
    public override void _Ready()
    {
        Animator = new Animator(_sprite, _bloodEffects, AttackMarker);
        MovementController = new MovementController(this, InputManager.GetContext<ControlContext>());
        StateMachine = new StateMachine<Character>(this);
        StateMachine.ChangeState(new IdleState(this));
        
        _sprite.Material = _materialOutline;
    }
    
    public override void _Process(double delta)
    {
        MovementController.UpdateDirection(InputVector);
        Animator.UpdateSpriteDirection(MovementController.FacingDirection);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        StateMachine.PhysicsUpdate(delta);
    }
    
    public void UpdateGravity(double delta)
    {
        MovementController.ApplyGravity(delta);
    }
    
    public void SetControlMode(ControlMode mode)
    {
        ControlMode = mode;
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;

        var effectsLayer = GetTree().CurrentScene.GetNodeOrNull<Node>("Level/EffectsLayer");
        
        GD.Print(effectsLayer);
        
        if (effectsLayer != null)
            Animator.PlayBloodEffect(effectsLayer, Position);
    }
    
    public void Jump() => MovementController.HandeJump();
}