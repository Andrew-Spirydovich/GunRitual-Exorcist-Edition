using System;
using System.Threading.Tasks;
using Godot;

public class PlayerAnimator
{
    private readonly AnimatedSprite2D _sprite;
    private readonly Marker2D _weaponMarker;
    private Vector2 _defaultMarkerPos;
    
    public PlayerAnimator(AnimatedSprite2D sprite, Marker2D weaponMarker)
    {
        _sprite = sprite;
        _weaponMarker = weaponMarker;
        _defaultMarkerPos = weaponMarker.Position;
    }

    public void SetDefaultMarker(Vector2 position)
    {
        _defaultMarkerPos = position;
    }

    public void SetAnimation(string name)
    {
        if (_sprite.Animation == name && _sprite.IsPlaying())
            return;
        
        _sprite.Play(name);
    }
    
    public void ConnectAnimationFinished(Action callback)
    {
        _sprite.AnimationFinished += callback;
    }
    
    public void DisconnectAnimationFinished(Action callback)
    {
        _sprite.AnimationFinished -= callback;
    }
    
    public void UpdateSpriteDirection(Vector2 playerDirection)
    {
        var isFacingLeft = playerDirection != Vector2.Right;
        _sprite.FlipH = isFacingLeft;
        
        //GD.Print(_weaponMarker.Position);
        
        _weaponMarker.Position = isFacingLeft
            ? new Vector2(-_defaultMarkerPos.X, _defaultMarkerPos.Y)
            : _defaultMarkerPos;
    }
}
