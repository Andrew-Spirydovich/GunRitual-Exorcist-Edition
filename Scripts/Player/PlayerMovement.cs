using Godot;
using GunRitualExorcistEdition.Scripts.Core;

public class PlayerMovement
{
    private const float RUN_SPEED = 200f;
    private const float GRAVITY = 900f;
    private const float ROLL_FORCE = 250f;
    private const float SLIDE_SPEED = 400f;
    private const float JUMP_FORCE = 400f;
    public Vector2 FacingDirection { get; private set; } = Vector2.Right;
    
    private readonly Player _player;
    private readonly ControlContext _controlContext;
    
    public PlayerMovement(Player player, ControlContext context)
    {
        _player = player;
        _controlContext = context;
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
        var velocity = FacingDirection * ROLL_FORCE;
        _player.Velocity = new Vector2(velocity.X, _player.Velocity.Y);
    }

    public void HandeSlide()
    {
        _player.Velocity = FacingDirection * SLIDE_SPEED;
    }

    public void HandeJump()
    {
        var vel = _player.Velocity;
        vel.Y = -JUMP_FORCE;
        _player.Velocity = vel;
    }
    
    public void ApplyGravity(double delta)
    {
        var vel = _player.Velocity;
        vel.Y += GRAVITY * (float)delta;
        _player.Velocity = vel;
    }
    
    public Vector2 GetInputDirection()
    {
        var direction = _controlContext.MoveDirection;
        
        return direction.Length() > 1 ? direction.Normalized() : direction;
    }
    
    public void UpdateDirection(Vector2 input)
    {
        if (input.Length() > 0.01f)
            FacingDirection = input.Normalized();
    }
}
