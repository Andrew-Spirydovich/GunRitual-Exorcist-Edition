using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Player.States;

public class StateMachine
{
    public PlayerState CurrentState { get; private set; }
    
    private Player _player;

    public StateMachine(Player player)
    {
        _player = player;
    }
    
    public void ChangeState(PlayerStateType stateType)
    {
        CurrentState?.Exit();
        
        CurrentState = stateType switch
        {
            PlayerStateType.Idle => new IdleState(_player),
            PlayerStateType.Run => new RunState(_player),
            PlayerStateType.Jump => new JumpState(_player),
            PlayerStateType.Fall => new FallState(_player),
            PlayerStateType.Roll => new RollState(_player),
            PlayerStateType.Slide => new SlideState(_player),
            PlayerStateType.Land => new LandState(_player),
            _ => CurrentState
        };

        if (!_player.IsLocal)
            GD.Print("Поставили состояние не локальному игроку {}", CurrentState);
        
        if (CurrentState != null)
        {
            CurrentState.Enter();
            
            if(!_player.IsLocal)
                return;
            
            var network = NetworkClient.Instance;
            network.SendStateRequest(network.LocalUserID, stateType.ToString());
        }
    }

    public void Update(double delta)
    {
        CurrentState?.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }
}
