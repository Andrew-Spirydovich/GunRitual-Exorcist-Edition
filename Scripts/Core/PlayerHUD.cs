using Godot;
using GunRitualExorcistEdition.Scripts.Items.Wepons;

public partial class PlayerHUD : CanvasLayer
{
    [Export] private TextureProgressBar _healthBar;
    [Export] private TextureRect _weaponSprite;
    [Export] private Label _weaponAmmo;
    [Export] private Label _weaponName;
    [Export] private Godot.Collections.Dictionary<WeaponType, CompressedTexture2D> _weaponSprites;

    public override void _Ready()
    {
        Visible = true;
        UpdateWeapon(new Unarmed());
    }
    
    public void ConnectToPlayer(Player player)
    {
        var inventory = player.Inventory;
        
         if (inventory != null)
         {
             inventory.OnWeaponChanged += UpdateWeapon;
             player.OnAmmoChanged += UpdateAmmo;
         }
        
    }

    public void UpdateHealth(float health)
    {
        var tween = CreateTween();
        tween.TweenProperty(_healthBar, "value", health, 0.3f);
    }
    
    public void UpdateWeapon(Weapon weapon)
    {
        if (weapon.Type == WeaponType.Unarmed)
        {
            _weaponSprite.Visible = false;
            _weaponAmmo.Visible = false;
            _weaponName.Visible = false;
        }
        else
        {
            _weaponSprite.Visible = true;
            _weaponAmmo.Visible = true;
            _weaponName.Visible = true;
            _weaponSprite.Texture = _weaponSprites[weapon.Type];
            _weaponAmmo.Text = $"{weapon.CurrentAmmo}/{weapon.MaxAmmo}";
            _weaponName.Text = $"{weapon.Type}";
        }
    }
    
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        _weaponAmmo.Text = $"{currentAmmo}/{maxAmmo}";
    }
}
