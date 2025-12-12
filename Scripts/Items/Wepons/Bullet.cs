using System.Collections.Generic;
using Godot;

namespace GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 600f;
    [Export] public Line2D Trail;
    public string BulletId { get; set; }
    public string OwnerId { get; set; }
    
    private int _maxPoints = 10;
    private Queue<Vector2> _points = new Queue<Vector2>();
    public Vector2 Direction { get; set; } = Vector2.Right;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
        _points.Enqueue(Position);
        
        if (_points.Count > _maxPoints)
            _points.Dequeue();
        
        Trail.ClearPoints();
        
        foreach (var point in _points)
            Trail.AddPoint(point);
    }

    private void OnBodyEntered(Node2D body)
    {

        if (body is global::Player target && !string.IsNullOrEmpty(target.NetworkId))
        {
            GD.Print(BulletId);
            GD.Print(OwnerId);
            GD.Print(target.NetworkId);
            NetworkClient.Instance.SendBulletHitRequest(
                BulletId,
                OwnerId, 
                target.NetworkId);
        }
    }
}