using System.Collections.Generic;
using Godot;

using GunRitualExorcistEdition.Scripts.Characters.States;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Core.Network;

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
    public float Health { get; private set; } = 100f;
    public Vector2 InputVector { get; protected set; }

    private CharacterNetworkSync _networkSync;
    
    private Dictionary<PlayerStateType, State<Character>> _stateMap;
    public bool IsDead { get; private set; }
    
    public void InitializeStateMap()
    {
        _stateMap = new Dictionary<PlayerStateType, State<Character>>
        {
            { PlayerStateType.IdleState, new IdleState(this) },
            { PlayerStateType.RunState, new RunState(this) },
            { PlayerStateType.JumpState, new JumpState(this) },
            { PlayerStateType.FallState, new FallState(this) },
            { PlayerStateType.LandState, new LandState(this) },
            { PlayerStateType.RollState, new RollState(this) },
            { PlayerStateType.ShootState, new ShootState(this) },
            { PlayerStateType.SlideState, new SlideState(this) }
        };
    }

    public State<Character>? MapEnumToState(PlayerStateType stateType)
        => _stateMap.TryGetValue(stateType, out var state) ? state : null;
    
    public override void _Ready()
    {
        _networkSync = new CharacterNetworkSync();
        _networkSync.Initialize(this);
        
        Animator = new Animator(_sprite, _bloodEffects, AttackMarker);
        
        MovementController = ControlMode == ControlMode.Local ? 
            new MovementController(this, InputManager.GetContext<ControlContext>()) : 
            new MovementController(this, null);
        
        StateMachine = new StateMachine<Character>(this);
        StateMachine.ChangeState(new IdleState(this));
        
        _sprite.Material = _materialOutline;
    }
    
    public override void _Process(double delta)
    {
        if(ControlMode != ControlMode.Remote)
            StateMachine.Update(delta);
        
        MovementController.UpdateDirection(InputVector);
        Animator.UpdateSpriteDirection(MovementController.FacingDirection);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        StateMachine.PhysicsUpdate(delta);
        
        MovementController.HandleHorizontalMovement();
            
        _networkSync.Tick(delta);
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
        if (IsDead)
            return;

        Health -= damage;

        var effectsLayer = GetTree().CurrentScene
            .GetNodeOrNull<Node>("Level/EffectsLayer");

        if (effectsLayer != null)
            Animator.PlayBloodEffect(effectsLayer, Position);

        if (Health <= 0)
            Die();
    }
    
    public void Respawn(Vector2 position, float health)
    {
        IsDead = false;
        Health = health;

        Position = position;
        Velocity = Vector2.Zero;

        Show();

        // Включаем физику
        SetProcess(true);
        SetPhysicsProcess(true);

        // Включаем коллайдер, чтобы пули регались
        if (_collider != null)
            _collider.Disabled = false;

        // Если есть Area2D для выстрелов
        var area = GetNodeOrNull<Area2D>("CollisionShape2D");
        if (area != null)
            area.Monitoring = true;
    }
    
    protected virtual void Die()
    {
        IsDead = true;
        Health = 0;

        SetProcess(false);
        SetPhysicsProcess(false);

        // Отключаем коллайдер / Area2D
        if (_collider != null)
            _collider.Disabled = true;

        var area = GetNodeOrNull<Area2D>("CollisionShape2D");
        if (area != null)
            area.Monitoring = false;

        Hide();
    }

    public void Jump()
    {
        MovementController.HandeJump();
        
        if (ControlMode == ControlMode.Local)
            _networkSync.SendImmediate();
    }
    
    public void OnLocalStateChanged(string state)
    {
        _networkSync.SendStateImmediate();
    }
}