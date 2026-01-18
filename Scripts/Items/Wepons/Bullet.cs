    using System.Collections.Generic;
    using Godot;

    namespace GunRitualExorcistEdition.Scripts.Items.Wepons;

    public partial class Bullet : Area2D
    {
        [Export] public float Speed = 600f;
        [Export] public Line2D Trail;

        public string BulletId { get; set; }
        public string OwnerId { get; set; }
        public Vector2 Direction { get; set; }

        private bool _hit;
        private readonly Queue<Vector2> _points = new();
        private const int MaxPoints = 10;

        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        public override void _PhysicsProcess(double delta)
        {
            GlobalPosition += Direction.Normalized() * Speed * (float)delta;

            _points.Enqueue(GlobalPosition);
            if (_points.Count > MaxPoints)
                _points.Dequeue();

            Trail.ClearPoints();
            foreach (var p in _points)
                Trail.AddPoint(p);
        }

        private void OnBodyEntered(Node2D body)
        {
            if (_hit)
                return;

            if (body is global::Player target)
            {
                if (target.NetworkId == OwnerId)
                    return;

                _hit = true;

                NetworkClient.Instance.SendBulletHitRequest(
                    BulletId,
                    OwnerId,
                    target.NetworkId
                );

                SetPhysicsProcess(false);
                Hide();
                // QueueFree();
            }
        }
    }
