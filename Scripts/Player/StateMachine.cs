using Godot;
using System;

public class StateMachine
{
    private PlayerState _currentState;

    public void ChangeState(PlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update(double delta)
    {
        _currentState?.Update(delta);
    }

    public void PhysicsUpdate(double delta)
    {
        _currentState?.PhysicsUpdate(delta);
    }
}
