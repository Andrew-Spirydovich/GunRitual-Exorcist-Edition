using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class MovementController
{
    private const float RUN_SPEED = 200f;
    private const float GRAVITY = 900f;
    private const float ROLL_FORCE = 150f;
    private const float SLIDE_SPEED = 350f;
    private const float JUMP_FORCE = 400f;
    public Vector2 FacingDirection { get; private set; } = Vector2.Right;
    public Vector2 GlobalPosition => _character.GlobalPosition;
    
    private readonly Character _character;
    private readonly ControlContext _controlContext;
    
    public MovementController(Character character, ControlContext context)
    {
        _character = character;
        _controlContext = context;
    }
    
    public void HandleHorizontalMovement()
    {
        var input = GetInputDirection();
        var velocity = _character.Velocity;
        velocity.X = input.X * RUN_SPEED;
        
        _character.Velocity = velocity;
    }

    public void HandleRoll()
    {
        var velocity = FacingDirection * ROLL_FORCE;
        _character.Velocity = new Vector2(velocity.X, _character.Velocity.Y);
    }

    public void HandeSlide()
    {
        _character.Velocity = FacingDirection * SLIDE_SPEED;
    }

    public void HandeJump()
    {
        var vel = _character.Velocity;
        vel.Y = -JUMP_FORCE;
        _character.Velocity = vel;
    }
    
    public void ApplyGravity(double delta)
    {
        var vel = _character.Velocity;
        vel.Y += GRAVITY * (float)delta;
        _character.Velocity = vel;
        _character.MoveAndSlide();
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
