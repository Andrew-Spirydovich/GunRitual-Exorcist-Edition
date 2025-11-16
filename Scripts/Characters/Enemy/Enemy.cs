using Godot;
using GunRitualExorcistEdition.Scripts.Player;

namespace GunRitualExorcistEdition.Scripts.Characters.Enemy;

public partial class Enemy : Character, IArmedAttacker
{
    private EnemyAI _ai;
    
    public override void _Ready()
    {
        base._Ready();

        _ai = new EnemyAI(this);
        SetControlMode(ControlMode.AI);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _ai.Update(delta);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        GD.Print("Enemy hit! HP left: " + Health);
    }

    public Weapon CurrentWeapon { get; }
    public State<Character> Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Reload()
    {
        throw new System.NotImplementedException();
    }

    public void PickUpWeapon(Weapon weapon)
    {
        throw new System.NotImplementedException();
    }
}