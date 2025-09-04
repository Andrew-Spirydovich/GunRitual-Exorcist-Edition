using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Revolver : Weapon
{
    public Revolver()
    {
        Type = WeaponType.Revolver;
        CurrentAmmo = MaxAmmo =  10;
        Damage = 20f;
    }

    public override void Attack(Player player)
    {
        if (CurrentAmmo <= 0) return;

        CurrentAmmo--;
        GD.Print($"{Type} fired! Ammo left: {CurrentAmmo}");

        // тут можно спавнить пулю в мире через Player
        // например player.SpawnBullet()
    }
}