using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public abstract partial class Weapon
{
    [Export] public WeaponType Type { get; private protected set; }
    [Export] public int CurrentAmmo { get; private protected set; }
    [Export] public int MaxAmmo { get; private protected set; }
    [Export] public float FireRate { get; private protected set; }
    [Export] public float Damage { get; private protected set; }
    
    protected readonly PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
    protected AudioStreamMP3 ShootSound;
    public event Action<int, int> AmmoChanged;

    protected Weapon(WeaponType type, int maxAmmo, float fireRate, float damage)
    {
        Type = type;
        CurrentAmmo = MaxAmmo = maxAmmo;
        FireRate = fireRate;
        Damage = damage;
    }
    
    public abstract void Attack(Node world, Vector2 position, Vector2 direction);
    
    protected void UseAmmo()
    {
        if (CurrentAmmo <= 0) 
            return;
        
        CurrentAmmo--;
        AmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }
    public override string ToString() => $"{Type} (Ammo: {CurrentAmmo})";
}