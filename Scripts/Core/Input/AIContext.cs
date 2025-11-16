using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public class AIContext : InputContext
{
    public Vector2 MoveDirection { get; set; } = Vector2.Zero;
    public bool ShouldFire { get; set; } = false;
    public bool ShouldJump { get; set; } = false;
    public bool ShouldReload { get; set; } = false;
}