using Godot;
using System;

public class PlayerMovement
{
    private const float _speed = 200f;
    private CollisionShape2D _collisionShape;
    private Player _player;
    public PlayerMovement(CollisionShape2D collisionShape, Player player)
    {
        _collisionShape = collisionShape;
        _player = player;
    }

    public void Move(Node2D target, double delta)
    {
        var direction = GetInputDirection();
        var velocity = direction * _speed;

        if (target is CharacterBody2D character)
        {
            character.Velocity = velocity;
            character.MoveAndSlide();
        }
        else
        {
            target.Position += velocity * (float)delta;
        }
    }
    
    public Vector2 GetInputDirection()
    {
        if (!_player.IsLocal)
            return Vector2.Zero;
        
        var x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        var y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        var dir = new Vector2(x, y);
        
        return dir.Length() > 1 ? dir.Normalized() : dir;
    }
}
