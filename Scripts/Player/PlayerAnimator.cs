using System;
using Godot;

public class PlayerAnimator
{
    private readonly AnimatedSprite2D _sprite;
    private Action _animationFinishedCallback;
    
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
    
    public void ConnectAnimationFinished(Action callback)
    {
        _animationFinishedCallback = callback;
        _sprite.AnimationFinished += _animationFinishedCallback;
    }
    
    public void DisconnectAnimationFinished(Action callback)
    {
        if (_animationFinishedCallback != null)
        {
            _sprite.AnimationFinished -= _animationFinishedCallback;
            _animationFinishedCallback = null;
        }
    }
    
    public void UpdateDirection(bool facingRight)
    {
        _sprite.FlipH = !facingRight;
    }
}
