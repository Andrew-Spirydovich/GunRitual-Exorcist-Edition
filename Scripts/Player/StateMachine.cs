using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player.States;

public class StateMachine
{
    public PlayerState CurrentState { get; private set; }
    
    private Player _player;

    public StateMachine(Player player)
    {
        _player = player;
    }
    
    public void ChangeState(PlayerState nextState)
    {
        CurrentState?.Exit();
        CurrentState = nextState;

        if (!_player.IsLocal)
            GD.Print("Поставили состояние не локальному игроку {}", CurrentState);
        
        if (CurrentState != null)
        {
            CurrentState.Enter();
            
            if(!_player.IsLocal)
                return;
            
            var network = NetworkClient.Instance;
            network.SendStateRequest(network.LocalUserID, CurrentState.ToString());
        }
    }

    public void Update(double delta)
    {
        CurrentState?.Update(delta);
        
        var nextState = CurrentState?.CheckTransitions(InputManager.GetContext<ControlContext>());
        
        if(nextState != null)
            ChangeState(nextState);
    }

    public void PhysicsUpdate(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }
}
