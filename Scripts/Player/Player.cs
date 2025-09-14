using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public partial class Player : CharacterBody2D
{
    [Export] private CollisionShape2D _collider;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private Camera2D _camera;
    [Export] private Material _material;
    [Export] private Label _playerLabel;
    [Export] private Marker2D _weaponMarker;
    
    public bool IsLocal { get; set; }
    public StateMachine StateMachine { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public InventoryManager Inventory { get; private set; }
    public Vector2 InputVector { get; private set; }
    public event Action<int, int> OnAmmoChanged;
    
    public override void _Ready()
    {
        Movement = new PlayerMovement(this);
        Animator = new PlayerAnimator(_sprite);
        StateMachine = new StateMachine(this);
        Inventory = new InventoryManager(_weaponMarker);
        StateMachine.ChangeState(PlayerStateType.Idle);
        SetProcess(true);

        if (IsLocal)
        {
            _sprite.Material = _material;
            _camera.Enabled = true;
            
            var ui = GetTree().Root.GetNode<PlayerUi>("GameScene/PlayerUI"); // поправь путь
            ui.ConnectToPlayer(this);
        }
    }   

    public override void _Process(double delta)
    {
        if (IsLocal)
        {
            InputVector = Movement.GetInputDirection();
            StateMachine.Update(delta);
        }
        
        //GD.Print($"{StateMachine.CurrentState}");
        
        Movement.UpdateDirection(InputVector);
        Animator.UpdateDirection(Movement.FacingRight);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (IsLocal)
            StateMachine.PhysicsUpdate(delta);
    }

    public void SetDisplayName(string name)
    {
        _playerLabel.Text = name;
    }

    public void SetRemoteInput(Vector2 direction, Vector2 position, Vector2 velocity)
    {
        InputVector = direction;
        Position = position;
        Velocity = velocity;
    }
    
    public void PickUpWeapon(Weapon weapon)
    {
        Inventory.AddWeapon(weapon);
        
        weapon.AmmoChanged += (current, max) =>
        {
            OnAmmoChanged?.Invoke(current, max);
        };

        OnAmmoChanged?.Invoke(weapon.CurrentAmmo, weapon.MaxAmmo);
    }

    public void Attack()
    {
        var factingDirection = Movement.FacingRight ? Vector2.Right : Vector2.Left;
        Inventory.CurrentWeapon?.Attack(GetTree().CurrentScene, _weaponMarker.GlobalPosition, factingDirection);
    }
}
