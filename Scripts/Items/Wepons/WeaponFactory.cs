namespace GunRitualExorcistEdition.Scripts.Items.Wepons;

public static class WeaponFactory
{
    public static Weapon Create(WeaponType type)
    {
        return type switch
        {
            WeaponType.Revolver => new Revolver(),
            WeaponType.Ithaca37 => new Ithaca37(),
            _ => null
        };
    }
}