using System.Linq;
using Godot;

namespace GunRitualExorcistEdition.Scripts.Characters.Enemy;

public class EnemyAI
{
    private readonly Enemy _enemy;
    private float _detectionRadius = 300f;
    private float _attackDistance = 60f;
    private float _moveSpeed = 60f;

    private Timer _attackCooldown;

    public EnemyAI(Enemy enemy)
    {
        _enemy = enemy;

        _attackCooldown = new Timer
        {
            WaitTime = 1.2f,
            OneShot = true,
            Autostart = false
        };
        _enemy.AddChild(_attackCooldown);
    }

    public void Update(double delta)
    {
        if (_enemy.Health <= 0)
            return;

        // Находим ближайшего игрока
        var target = GetClosestPlayer();
        if (target == null)
        {
            _enemy.Animator.SetAnimation("Idle");
            _enemy.Velocity = Vector2.Zero;
            return;
        }

        Vector2 toTarget = target.GlobalPosition - _enemy.GlobalPosition;
        float distance = toTarget.Length();

        if (distance > _detectionRadius)
        {
            _enemy.Animator.SetAnimation("Idle");
            _enemy.Velocity = Vector2.Zero;
            return;
        }

        if (distance > _attackDistance)
        {
            MoveToward(toTarget.Normalized());
            _enemy.Animator.SetAnimation("Walk");
        }
        else
        {
            _enemy.Velocity = Vector2.Zero;
            TryAttack();
        }
    }

    private void MoveToward(Vector2 direction)
    {
        _enemy.Velocity = direction * _moveSpeed;
        _enemy.MoveAndSlide();
        _enemy.Animator.UpdateSpriteDirection(direction);
    }

    private void TryAttack()
    {
        if (_attackCooldown.TimeLeft > 0)
        {
            _enemy.Animator.SetAnimation("Idle");
            return;
        }

        _enemy.Animator.SetAnimation("Attack");
        _enemy.Attack();
        _attackCooldown.Start();
    }

    private Character GetClosestPlayer()
    {
        // ⚠️ Предполагаем, что все игроки находятся под "Level/Players"
        var playersNode = _enemy.GetTree().CurrentScene.GetNodeOrNull<Node>("PlayerManager/");
        if (playersNode == null)
            return null;

        var players = playersNode.GetChildren()
            .OfType<global::Player>()
            .Where(p => p.Health > 0)
            .ToList();

        if (players.Count == 0)
            return null;

        Character closest = null;
        float closestDistance = float.MaxValue;

        foreach (var player in players)
        {
            float dist = _enemy.GlobalPosition.DistanceTo(player.GlobalPosition);
            if (dist < closestDistance)
            {
                closest = player;
                closestDistance = dist;
            }
        }

        return closest;
    }
}