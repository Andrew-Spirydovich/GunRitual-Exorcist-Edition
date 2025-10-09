using Godot;

public class PlayerMovement
{
    private const float RUN_SPEED = 200f;
    private const float GRAVITY = 900f;
    private const float ROLL_SPEED = 300f;
    private const float SLIDE_SPEED = 400f;
    
    public Vector2 FacingDirection { get; private set; } = Vector2.Right;
    
    private readonly Player _player;
            
    public PlayerMovement(Player player)
    {
        _player = player;
    }
    
    public bool IsOnFloor() => _player.IsOnFloor();
    
    public void HandleHorizontalMovement()
    {
        var input = GetInputDirection();
        var velocity = _player.Velocity;
        velocity.X = input.X * RUN_SPEED;
        
        _player.Velocity = velocity;
        _player.MoveAndSlide();
    }

    public void HandleRoll()
    {
        var velocity = FacingDirection * ROLL_SPEED;
        _player.Velocity = new Vector2(velocity.X, _player.Velocity.Y);
    }

    public void HandeSlide()
    {
        _player.Velocity = FacingDirection * SLIDE_SPEED;
    }
    
    public void ApplyGravity(double delta)
    {
        var vel = _player.Velocity;
        vel.Y += GRAVITY * (float)delta;
        _player.Velocity = vel;
    }
    
    public Vector2 GetInputDirection()
    {
        var x = Input.GetActionStrength("input_right") - Input.GetActionStrength("input_left");
        var y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        var direction = new Vector2(x, y);
        
        return direction.Length() > 1 ? direction.Normalized() : direction;
    }
    
    public void UpdateDirection(Vector2 input)
    {
        if (input.Length() > 0.01f)
            FacingDirection = input.Normalized();
    }
}
