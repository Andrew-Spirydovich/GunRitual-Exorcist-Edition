using System;
using Godot;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class WeaponLoot() : Area2D
{
    [Export] private WeaponType _weaponType;
    [Export] private Sprite2D _sprite;
    [Export] private AudioStream _pickupSound;
    
    public override void _Ready()
    {
        UpdateSprite();
        BodyEntered += OnBodyEntered;   
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            AudioManager.Instance.PlaySFX(_pickupSound);
            
            var weapon = WeaponFactory.Create(_weaponType);
            player.PickUpWeapon(weapon);
            
            QueueFree();
        }
    }

    private void UpdateSprite()
    {
        var texturePath = _weaponType switch
        {
            WeaponType.Revolver => "res://Assets/Sprites/Items/revolver.png",
            WeaponType.Ithaca37 => "res://Assets/Sprites/Items/Ithaca 37.png",
            _ => "res://Assets/Weapons/default.png"
        };

        _sprite.Texture = GD.Load<Texture2D>(texturePath);
    }
}