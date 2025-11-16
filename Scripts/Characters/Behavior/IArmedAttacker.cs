namespace GunRitualExorcistEdition.Scripts.Player;

public interface IArmedAttacker: IAttacker
{
    Weapon CurrentWeapon { get; }
    void Reload();
    void PickUpWeapon(Weapon weapon);
    
}