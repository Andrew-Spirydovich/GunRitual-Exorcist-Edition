using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;
using GunRitualExorcistEdition.Scripts.Player;

public partial class Player : Character, IArmedAttacker
{
    [Export] private Camera2D _camera;
    [Export] private Label _playerLabel;

    public InventoryManager Inventory { get; private set; }
    public Weapon CurrentWeapon => Inventory.CurrentWeapon;
    public string NetworkId { get; set; }
    public event Action<int, int> OnAmmoChanged;
    public event Action<float> OnHealthChanged;
    
    public override void _Ready()
    {
        base._Ready();
        Inventory = new InventoryManager();
        
        if (ControlMode == ControlMode.Local)
        {
            _camera.Enabled = true;
            
            var ui = GetTree().Root.GetNode<PlayerHUD>("GameScene/PlayerUI");
            ui.ConnectToPlayer(this);
        }
    }   

    public override void _Process(double delta)
    {
        if (ControlMode == ControlMode.Local)
        {
            InputVector = MovementController.GetInputDirection();
        }
        
        base._Process(delta);
    }

    public void SetNetworkId(string id)
    {
        NetworkId = id;
    }

    public void SetDisplayName(string name)
    {
        _playerLabel.Text = name;
    }
    
    public string GetDisplayName() => _playerLabel.Text;

    public void SetRemoteInput(Vector2 direction, Vector2 position, Vector2 velocity)
    {
        InputVector = direction;
        Position = position;
        Velocity = velocity;
    }
    
    public void PickUpWeapon(Weapon weapon)
    {
        Inventory.AddWeapon(weapon);

        Animator.SetDefaultMarker(weapon.MarkerPosition);
        
        weapon.AmmoChanged += (current, max) =>
        {
            OnAmmoChanged?.Invoke(current, max);
        };

        OnAmmoChanged?.Invoke(weapon.CurrentAmmo, weapon.MaxAmmo);
    }
    
    protected override void Die()
    {
        base.Die();

        if (IsLocalPlayer)
            UIManager.Instance.ShowRespawn();
    }
    
    public void RespawnPlayer(Vector2 position, float health)
    {
        Respawn(position, health);
        OnHealthChanged?.Invoke(Health);
    }

    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        OnHealthChanged?.Invoke(Health);
    }
    
    public State<Character> Attack()
    {
        var currentWeapon = Inventory.CurrentWeapon;

        if (currentWeapon.Type == WeaponType.Unarmed) 
            return new IdleState(this);
        
        if(currentWeapon.TryAttack(GetTree().CurrentScene, 
               AttackMarker.GlobalPosition, 
               MovementController.FacingDirection) == true)
            return new ShootState(this);

        return null;
    }

    public void Reload()
    {
        var currentWeapon = Inventory.CurrentWeapon;
        currentWeapon?.Reload(this);
    }
    
    public bool IsLocalPlayer =>
        NetworkClient.Instance != null &&
        NetworkClient.Instance.LocalUserID == NetworkId;
    
}
