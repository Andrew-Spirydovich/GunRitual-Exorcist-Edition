using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public abstract partial class Weapon
{
    [Export] public WeaponType Type { get; private protected set; }
    [Export] public int CurrentAmmo { get; private protected set; }
    [Export] public int MaxAmmo { get; private protected set; }
    [Export] public float FireRate { get; private protected set; }
    [Export] public float Damage { get; private protected set; }

    protected Weapon(WeaponType type, int maxAmmo, float fireRate, float damage)
    {
        Type = type;
        CurrentAmmo = MaxAmmo = maxAmmo;
        FireRate = fireRate;
        Damage = damage;
    }
    
    public abstract void Attack(Player player);

    public override string ToString() => $"{Type} (Ammo: {CurrentAmmo})";
}