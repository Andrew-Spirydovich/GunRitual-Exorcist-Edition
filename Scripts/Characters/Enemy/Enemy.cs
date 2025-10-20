using Godot;
using GunRitualExorcistEdition.Scripts.Player;

namespace GunRitualExorcistEdition.Scripts.Characters.Enemy;

public partial class Enemy : Character
{
    public override void _Ready()
    {
        base._Ready();
        SetControlMode(ControlMode.AI);
    }   
    
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        GD.Print("Enemy hit! HP left: " + Health);
    }
}