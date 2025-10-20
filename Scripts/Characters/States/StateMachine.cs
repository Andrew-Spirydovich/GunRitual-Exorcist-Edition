using Godot;
using System;
using GunRitualExorcistEdition.Scripts.Characters;
using GunRitualExorcistEdition.Scripts.Core;
using GunRitualExorcistEdition.Scripts.Player;
using Character = GunRitualExorcistEdition.Scripts.Characters.Character;

public class StateMachine<T> where T : Character
{
    private State<T> _currentState;
    private readonly T _entity;

    public StateMachine(T entity)
    {
        _entity = entity;
    }
    
    public void ChangeState(State<T> nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;

        if (_entity.ControlMode != ControlMode.Local)
            GD.Print("Поставили состояние не локальному игроку {}", _currentState);
        
        if (_currentState != null)
        {
            _currentState.Enter();
            
            if(_entity.ControlMode != ControlMode.Local)
                return;
            
            var network = NetworkClient.Instance;
            network.SendStateRequest(network.LocalUserID, _currentState.ToString());
        }
    }

    public void Update(double delta)
    {
        var nextState = _currentState?.CheckTransitions(InputManager.GetContext<ControlContext>());
        
        if(nextState != null)
            ChangeState(nextState);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentState?.PhysicsUpdate(delta);
    }
}
