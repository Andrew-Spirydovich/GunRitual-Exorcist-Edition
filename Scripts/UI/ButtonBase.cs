using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;

public partial class ButtonBase : Button
{
    [Export] private AudioStream _hoverSound;   
    [Export] private AudioStream _clickSound;
    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        Pressed += OnMousePressed;
    }

    private void OnMouseEntered()
    {
        AudioManager.Instance.PlaySFX(_hoverSound);
    }

    private void OnMousePressed()
    {
        AudioManager.Instance.PlaySFX(_clickSound);
    }
}
