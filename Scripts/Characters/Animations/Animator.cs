using System;
using System.Threading.Tasks;
using Godot;
using GunRitualExorcistEdition.Scripts;

public class Animator
{
    private readonly AnimatedSprite2D _sprite;
    private readonly PackedScene _bloodEffect;
    private readonly Marker2D _attackMarker;
    private Vector2 _defaultMarkerPos;
    
    public bool IsAnimationFinished { get; private set; }
    public event Action AnimationFinished;
    
    public Animator(AnimatedSprite2D sprite, PackedScene bloodEffect, Marker2D attackMarker)
    {
        _sprite = sprite;
        _bloodEffect = bloodEffect;
        _attackMarker = attackMarker;
        _defaultMarkerPos = attackMarker.Position;
        
        _sprite.AnimationFinished += OnAnimationFinished;
    }

    public void SetDefaultMarker(Vector2 position)
    {
        _defaultMarkerPos = position;
    }

    public void SetAnimation(string name)
    {
        if (_sprite.Animation == name && _sprite.IsPlaying())
            return;
        
        
        IsAnimationFinished = false;
        _sprite.Play(name);
    }

    private void OnAnimationFinished()
    {
        IsAnimationFinished = true;
        AnimationFinished?.Invoke();
    }
    
    public void UpdateSpriteDirection(Vector2 playerDirection)
    {
        var isFacingLeft = playerDirection != Vector2.Right;
        _sprite.FlipH = isFacingLeft;
        
        //GD.Print(_weaponMarker.Position);
        
        _attackMarker.Position = isFacingLeft
            ? new Vector2(-_defaultMarkerPos.X, _defaultMarkerPos.Y)
            : _defaultMarkerPos;
    }
    
    public void PlayBloodEffect(Node world, Vector2 position)
    {
        if (_bloodEffect == null)
            return;

        var blood = (BloodEffect)_bloodEffect.Instantiate();
        blood.GlobalPosition = position;
        world.AddChild(blood);
        blood.PlayRandom();
    }
}
