using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class ShootState : PlayerState
{
    private double _shootCooldown;
    private bool _animationFinished;

    public ShootState(Player player) : base(player) => AnimationName = "Shoot";
    
    public override void Enter()
    {
        PlayShootAnimation();
    }

    private void PlayShootAnimation()
    {
        // _player.Animator.SetAnimation(AnimationName + "Pistol");
        // _player.Animator.ConnectAnimationFinished(OnShootAnimationFinished);
        //
        // var weapon = _player.Inventory.CurrentWeapon;
        // _player.Attack();
        //
        // _shootCooldown = weapon.FireRate;
        // _animationFinished = false;
    }

    private void OnShootAnimationFinished()
    {
        _animationFinished = true;
    }

    // public override void Update(double delta)
    // {
        // if (_shootCooldown > 0)
        // {
        //     _shootCooldown -= delta;
        //     return; // ещё нельзя выходить или перезапускать анимацию
        // }
        //
        // // ждём окончания анимации
        // if (!_animationFinished)
        //     return;
        //
        // if (_player.IsLocal)
        // {
        //     var inputContext = InputManager.GetContext<ControlContext>();
        //
        //     if (inputContext != null)
        //     {
        //         // Если игрок снова нажал fire — перезапускаем стрельбу
        //         if (inputContext.IsFirePressed)
        //         {
        //             var weapon = _player.Inventory.CurrentWeapon;
        //             if (weapon != null && weapon.CurrentAmmo > 0)
        //             {
        //                 _player.Animator.DisconnectAnimationFinished(OnShootAnimationFinished);
        //                 PlayShootAnimation();
        //                 return;
        //             }
        //         }
        //
        //         // Иначе выходим в нужный стейт
        //         if (_player.InputVector == Vector2.Zero)
        //             _player.StateMachine.ChangeState(PlayerStateType.Idle);
        //         else
        //             _player.StateMachine.ChangeState(PlayerStateType.Run);
        //     }
        // }
    // }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        Player.Movement.ApplyGravity(delta);
        Player.MoveAndSlide();
        network.SendMoveRequest(network.LocalUserID, Player.GlobalPosition, Player.InputVector, Player.Velocity);
    }

    public override PlayerState CheckTransitions(ControlContext controlContext)
    {
        throw new NotImplementedException();
    }
}
