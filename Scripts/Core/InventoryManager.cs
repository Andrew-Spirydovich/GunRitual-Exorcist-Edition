using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public class InventoryManager
{
    public Weapon CurrentWeapon { get; private set; } = new Unarmed();
    public event Action<Weapon> OnWeaponChanged;

    public InventoryManager()
    {
    }
    
    public void AddWeapon(Weapon weapon)
    {

        CurrentWeapon = weapon;
        // GD.Print($"Подобрали {weapon}");
        OnWeaponChanged?.Invoke(CurrentWeapon); 
    }
}