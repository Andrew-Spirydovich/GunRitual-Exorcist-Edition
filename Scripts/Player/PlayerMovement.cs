using Godot;

public class PlayerMovement
{
    private const float SPEED = 200f;
    private readonly CollisionShape2D _collisionShape;
    private readonly Player _player;
            
    public PlayerMovement(CollisionShape2D collisionShape, Player player)
    {
        _collisionShape = collisionShape;
        _player = player;
    }

    public void Move(Node2D target, double delta)
    {
        var velocity = _player.InputVector * SPEED;

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
        var x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        var y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        var direction = new Vector2(x, y);
        
        return direction.Length() > 1 ? direction.Normalized() : direction;
    }
}
