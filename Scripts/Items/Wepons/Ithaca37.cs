using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Ithaca37 : Weapon
{
    public Ithaca37() : base(WeaponType.Ithaca37, 5, 0.5f,1,  30f)
    {
        ShootSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/winchester fire.mp3");
        ReloadSound = GD.Load<AudioStreamMP3>("res://Assets/Audio/winchester ready.mp3");
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
        
        AudioManager.Instance.PlaySFX(ShootSound);
        
        for (var i = 0; i < 3; i++)
        {
            var spreadDegrees = GD.RandRange(-5, 5);
            var spreadRadians = Mathf.DegToRad(spreadDegrees);
            var rotatedDirection = direction.Rotated(spreadRadians);
        
            var bulletDto = new BullletMoveDto
            {
                X = position.X,
                Y = position.Y,
                DirX = rotatedDirection.X,
                DirY = rotatedDirection.Y
            };
        
            NetworkClient.Instance.SendShootRequest(bulletDto);
            
            // var bullet = BulletScene.Instantiate<Bullet>();
            // bullet.GlobalPosition = position;
            // bullet.Direction = rotatedDirection.Normalized();
            // world.AddChild(bullet);
        }
        
        return true;
    }
}