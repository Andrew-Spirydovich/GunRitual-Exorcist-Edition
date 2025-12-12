using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class BulletManager : Node
{
    [Export] private PackedScene _bulletScene;
    private Godot.Collections.Dictionary<string, Bullet> _bullets = new();
    
    public void Spawn(string bulletId, string ownerId, Vector2 position, Vector2 direction)
    {
        if (_bullets.ContainsKey(bulletId))
            return;

        var bullet = _bulletScene.Instantiate<Bullet>();
        bullet.Name = $"Bullet_{bulletId}";
        bullet.OwnerId = ownerId;
        bullet.BulletId = bulletId;
        bullet.GlobalPosition = position;
        bullet.Direction = direction;
        
        AddChild(bullet);
        _bullets[bulletId] = bullet;
    }
    
    public void Remove(string bulletId)
    {
        if (!_bullets.TryGetValue(bulletId, out var bullet))
            return;

        bullet.QueueFree();
        _bullets.Remove(bulletId);
    }
}