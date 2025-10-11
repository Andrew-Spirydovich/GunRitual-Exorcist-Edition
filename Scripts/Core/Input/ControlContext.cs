using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public class ControlContext : InputContext
{
    public bool IsFirePressed => Input.IsActionJustPressed("input_fire");
    public bool IsJumpPressed => Input.IsActionJustPressed("input_jump");
    public bool IsRollPressed => Input.IsActionJustPressed("input_roll");
    public bool IsSlidePressed =>
        Input.IsActionPressed("input_down") &&
        (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
        Input.IsActionJustPressed("input_down");

    public bool IsReloadPressed => Input.IsActionJustPressed("input_reload");
    
    public Vector2 MoveDirection =>
        new Vector2(
            Input.GetActionStrength("input_right") - Input.GetActionStrength("input_left"),
            0
        );
}