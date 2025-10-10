using System;
using System.Threading.Tasks;
using Godot;

public class PlayerAnimator
{
    private readonly AnimatedSprite2D _sprite;
    public PlayerAnimator(AnimatedSprite2D sprite)
    {
        _sprite = sprite;
    }

    public void SetAnimation(string name)
    {
        if (_sprite.Animation == name && _sprite.IsPlaying())
            return;
        
        _sprite.Play(name);
    }

    // public double GetAnimationLength(string animationName)
    // {
    //     var frames = _sprite.SpriteFrames;
    //     var frameCount = frames.GetFrameCount(animationName);
    //
    //     var totalDuration = 0.0;
    //     
    //     for(var i = 0; i < frameCount; i++)
    //         totalDuration += frames.GetFrameDuration(animationName, i);
    //
    //     return totalDuration / _sprite.SpeedScale;
    // }
    
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
        var flipLeft = playerDirection != Vector2.Right;
        _sprite.FlipH = flipLeft;
    }
}
