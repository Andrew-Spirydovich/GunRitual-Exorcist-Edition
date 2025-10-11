using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public abstract class Weapon
{
    protected double LastFireTime;
    protected bool IsReloading;
    
    [Export] public WeaponType Type { get; private protected set; }
    [Export] public int CurrentAmmo { get; private protected set; }
    [Export] public int MaxAmmo { get; private protected set; }
    [Export] public float AttackSpeed { get; private protected set; }
    [Export] public float ReloadSpeed { get; private protected set; }
    [Export] public float Damage { get; private protected set; }
    
    public Vector2 MarkerPosition { get; private protected set; }
    
    protected readonly PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
    protected AudioStreamMP3 ShootSound;
    protected AudioStreamMP3 ReloadSound;
    protected AudioStreamMP3 EmptySound;
    public event Action<int, int> AmmoChanged;
    public event Action ReloadStarted;
    public event Action ReloadFinished;

    protected Weapon(WeaponType type, int maxAmmo, float attackSpeed, float reloadSpeed, float damage)
    {
        Type = type;
        CurrentAmmo = MaxAmmo = maxAmmo;
        AttackSpeed = attackSpeed;
        ReloadSpeed = reloadSpeed;
        Damage = damage;
    }

    public bool IsReadyToShoot()
    {
        var now = Time.GetTicksMsec() / 1000.0;
        return !IsReloading && now - LastFireTime >= AttackSpeed && CurrentAmmo > 0;
    }
    
    public abstract bool TryAttack(Node world, Vector2 position, Vector2 direction);

    public virtual async void Reload(Node world)
    {
        if (IsReloading || CurrentAmmo == MaxAmmo)
            return;

        IsReloading = true;
        AudioManager.Instance.PlaySFX(ReloadSound);
        ReloadStarted?.Invoke();

        GD.Print($"[{Type}] Reloading...");

        // создаём таймер на время перезарядки
        var timer = world.GetTree().CreateTimer(ReloadSpeed);
        await world.ToSignal(timer, "timeout"); // ← ждём, пока таймер сработает (не блокируя поток)

        CurrentAmmo = MaxAmmo;
        IsReloading = false;
        AmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
        ReloadFinished?.Invoke();

        GD.Print($"[{Type}] Reload complete!");
    }
    
    protected void UseAmmo()
    {
        if (CurrentAmmo <= 0) 
            return;
        
        CurrentAmmo--;
        AmmoChanged?.Invoke(CurrentAmmo, MaxAmmo);
    }
    public override string ToString() => $"{Type} (Ammo: {CurrentAmmo})";
}