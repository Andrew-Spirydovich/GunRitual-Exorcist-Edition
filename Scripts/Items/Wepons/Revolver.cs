using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Revolver : Weapon
{
    public Revolver() : base(WeaponType.Revolver, 10, 0.2f, 0.5f,20f)
    {
        ShootSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/Mauser C96 fire.mp3");
        ReloadSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/Mauser C96 reload.mp3");
        EmptySound = GD.Load<AudioStreamMP3>("res://Assets/Audio/No_Ammo_Sound_Effect.mp3");
        
        MarkerPosition = new Vector2(15, -7);
    }   

    public override bool TryAttack(Node world, Vector2 position, Vector2 direction)
    {
        var now = Time.GetTicksMsec() / 1000.0;
        
        if(IsReloading)
            return false;
        
        if (now - LastFireTime < AttackSpeed)
            return false;
        
        LastFireTime = now;

        if (CurrentAmmo <= 0)
        {
            AudioManager.Instance.PlaySFX(EmptySound);
            return false;
        }

        UseAmmo();
        var spreadDegrees = GD.RandRange(-5, 5);
        var spreadRadians = Mathf.DegToRad(spreadDegrees);
        var rotatedDirection = direction.Rotated(spreadRadians);
        
        var bullet = BulletScene.Instantiate<Bullet>();
        bullet.GlobalPosition = position;
        bullet.Direction = rotatedDirection.Normalized();
        AudioManager.Instance.PlaySFX(ShootSound);
        world.AddChild(bullet);
        
        return true;
    }
}