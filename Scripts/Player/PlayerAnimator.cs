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
    
    public void SetAnimation(string name, bool hasWeapon)
    {
        string animName = name;

        // Добавляем суффикс "_Weapon", если у игрока в руках оружие
        if (hasWeapon)
            animName = $"{name}Pistol";

        if (_sprite.Animation == animName && _sprite.IsPlaying())
            return;

        _sprite.Play(animName);
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
    
    public void UpdateSpriteDirection(Vector2 playerDirection)
    {
        var flipLeft = playerDirection != Vector2.Right;
        _sprite.FlipH = flipLeft;
    }
}
