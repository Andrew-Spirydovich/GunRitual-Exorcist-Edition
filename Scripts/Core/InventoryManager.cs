using System;
using System.Collections.Generic;
using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public class InventoryManager
{
    public Weapon CurrentWeapon { get; private set; }
    private Marker2D _marker;
    public event Action<Weapon> OnWeaponChanged;

    public InventoryManager(Marker2D marker)
    {
        _marker = marker;
    }
    
    public void AddWeapon(Weapon weapon)
    {
        switch (weapon.Type)
        {
            case WeaponType.Revolver: 
                _marker.Position = new Vector2(15, -7); 
                break;
        }
        
        CurrentWeapon = weapon;
        GD.Print($"Подобрали {weapon}");
        OnWeaponChanged?.Invoke(CurrentWeapon); 
    }
}