using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Ithaca37 : Weapon
{
    public Ithaca37() : base(WeaponType.Ithaca37, 5, 0.5f, 30f)
    {
        ShootSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/winchester fire.mp3");
    }

    public override void Attack(Node world, Vector2 position, Vector2 direction)
    {
        if (CurrentAmmo <= 0) 
            return;

        UseAmmo();
        
        var newDirection = direction.Normalized() + new Vector2(0, 2);
        
        for (var i = 0; i < 3; i++)
        {
            var bullet = BulletScene.Instantiate<Bullet>();
            bullet.GlobalPosition = position;
            bullet.Direction = newDirection - new Vector2(0, i * 2);
            AudioManager.Instance.PlaySFX(ShootSound);
            world.AddChild(bullet);
        }
    }
}