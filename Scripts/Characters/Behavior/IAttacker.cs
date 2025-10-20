namespace GunRitualExorcistEdition.Scripts.Player;

public interface IAttacker
{
    Weapon CurrentWeapon { get; }
    
    State<Characters.Character> Attack();
    void Reload();
    void PickUpWeapon(Weapon weapon);
    
}