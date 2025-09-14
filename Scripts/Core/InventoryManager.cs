using System;
using System.Collections.Generic;
using Godot;

public class InventoryManager
{
    public Weapon CurrentWeapon { get; private set; }
    
    public event Action<Weapon> OnWeaponChanged;
    
    public void AddWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;
        OnWeaponChanged?.Invoke(CurrentWeapon); 
    }
}