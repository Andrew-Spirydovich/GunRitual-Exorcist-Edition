using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public class PlayerInput : InputContext
{
    public bool FirePressed => Input.IsActionJustPressed("input_fire");
    public bool JumpPressed => Input.IsActionJustPressed("input_jump");
    public bool RollPressed => Input.IsActionJustPressed("input_roll");
    public bool SlidePressed =>
        Input.IsActionPressed("input_down") &&
        (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
        Input.IsActionJustPressed("input_down");

    public Vector2 MoveDirection =>
        new Vector2(
            Input.GetActionStrength("input_right") - Input.GetActionStrength("input_left"),
            0
        );
}