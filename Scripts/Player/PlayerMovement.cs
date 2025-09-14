using Godot;

public class PlayerMovement
{
    private const float SPEED = 200f;
    private const float GRAVITY = 900f;
    public bool FacingRight { get; private set; } = true;
    
    private readonly Player _player;
            
    public PlayerMovement(Player player)
    {
        _player = player;
    }
    
    public bool IsOnFloor() => _player.IsOnFloor();
    
    public void HandleHorizontalMovement(double delta)
    {
        var input = GetInputDirection();
        var velocity = _player.Velocity;
        velocity.X = input.X * SPEED;
        _player.Velocity = velocity;
        _player.MoveAndSlide();
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
        if (Mathf.Abs(input.X) > 0.01f)
        {
            FacingRight = input.X > 0;
        }
    }
}
