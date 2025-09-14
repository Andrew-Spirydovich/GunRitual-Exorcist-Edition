using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Ithaca37 : Weapon
{
    public Ithaca37() : base(WeaponType.Ithaca37, 5, 2f, 30f) { }

    public override void Attack(Player player)
    {
        if (CurrentAmmo <= 0) return;

        CurrentAmmo--;
        GD.Print($"{Type} fired! Ammo left: {CurrentAmmo}");

        // тут можно спавнить пулю в мире через Player
        // например player.SpawnBullet()
    }
}