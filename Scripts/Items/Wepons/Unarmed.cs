using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class Unarmed : Weapon
{
    public Unarmed() : base(WeaponType.Unarmed, 0, 0, 0,0f) { }

    public override bool TryAttack(Node world, Vector2 position, Vector2 direction)
    {
        throw new System.NotImplementedException();
    }
}