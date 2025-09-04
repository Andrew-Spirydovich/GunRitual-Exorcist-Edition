using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public abstract partial class Weapon
{
    [Export] protected WeaponType Type { get; private protected set; }
    [Export] protected int CurrentAmmo { get; private protected set; }
    [Export] public int MaxAmmo { get; private protected set; }
    [Export] public float FireRate { get; private protected set; }
    [Export] public float Damage { get; private protected set; }

    public abstract void Attack(Player player);

    public override string ToString() => $"{Type} (Ammo: {CurrentAmmo})";
}