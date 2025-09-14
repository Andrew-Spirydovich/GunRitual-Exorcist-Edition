using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Unarmed : Weapon
{
    public Unarmed() : base(WeaponType.Unarmed, 0, 0, 0f) { }

    public override void Attack(Player player)
    {
        GD.Print($"{Type} fired! Ammo left: {CurrentAmmo}");
    }
}