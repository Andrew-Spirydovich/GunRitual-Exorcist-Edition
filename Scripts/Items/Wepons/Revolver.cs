using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Revolver : Weapon
{
    public Revolver() : base(WeaponType.Revolver, 10, 0.2f, 20f)
    {
        ShootSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/Mauser C96 fire.mp3");
    }

    public override void Attack(Node world, Vector2 position, Vector2 direction)
    {
        if (CurrentAmmo <= 0) 
            return;

        UseAmmo();

        var bullet = BulletScene.Instantiate<Bullet>();
        bullet.GlobalPosition = position;
        bullet.Direction = direction.Normalized();
        AudioManager.Instance.PlaySFX(ShootSound);
        world.AddChild(bullet);
    }
}