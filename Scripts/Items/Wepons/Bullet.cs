using Godot;

namespace GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 600f;
    public Vector2 Direction { get; set; } = Vector2.Right;


    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
    }

    private void OnBodyEntered(Node2D body)
    {
        // if (body is Player player)
        // {
        //     player.TakeDamage(10);
        // }
        
        QueueFree();
    }
}