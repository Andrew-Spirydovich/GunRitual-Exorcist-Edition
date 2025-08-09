using Godot;
using System;

public class PlayerAnimator
{
    private AnimatedSprite2D _sprite;

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

    public void UpdateDirection(Vector2 input)
    {
        if (Mathf.Abs(input.X) > 0.01f)
            _sprite.FlipH = input.X < 0;
    }
}
