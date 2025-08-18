using Godot;
using System;

public class RunState : PlayerState
{
    private Player _player;

    public RunState(Player player)
    {
        _player = player;
    }
    
    public override void Enter()
    {
        _player.Animator.SetAnimation("Run");
    }   

    public override void Exit() { }

    public override void Update(double delta)
    {
        if (_player.InputVector == Vector2.Zero)
        {
            _player.StateMachine.ChangeState(new IdleState(_player));
        }

        if (Input.IsActionJustPressed("input_jump") && _player.IsOnFloor())
        {
            _player.StateMachine.ChangeState(new JumpState(_player));
        }
        
        if (Input.IsActionJustPressed("input_roll"))
        {
            _player.StateMachine.ChangeState(new RollState(_player));
        }
        
        if (_player.Velocity.Y > 0)
        {
            _player.StateMachine.ChangeState(new FallState(_player));
        }
        
        if (WantsToSlide() && _player.Movement.IsOnFloor())
        {
            _player.StateMachine.ChangeState(new SlideState(_player));
        }
    }
    
    public bool WantsToSlide()
    {
        return Input.IsActionPressed("input_down") &&
               (Input.IsActionPressed("input_left") || Input.IsActionPressed("input_right")) &&
               Input.IsActionJustPressed("input_down"); // триггер по дауну
    }

    public override void PhysicsUpdate(double delta)
    {
        var network = NetworkClient.Instance;
        _player.Movement.ApplyGravity(delta);
        _player.Movement.HandleHorizontalMovement(delta);
        network.SendMoveRequest(network.LocalUserID, _player.GlobalPosition, _player.InputVector);
    }
}
